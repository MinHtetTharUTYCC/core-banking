using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet;
using CoreBanking.Application.Common.Models;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Common.Behaviors;


public interface IIdempotentRequest
{
    string IdempotencyKey { get; }
}

public class IdempotencyBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    IDistributedLockFactory lockFactory
    ) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IIdempotentRequest
{
    private static readonly TimeSpan KeyTtl = TimeSpan.FromHours(24);
    private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(10);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = $"idempotency:{request.IdempotencyKey}";
        var lockResource = $"lock:idempotency:{request.IdempotencyKey}";

        // Quick cache check (no lock)
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<TResponse>(cached)!;

        // Acquire lock with retry
        using var redLock = await lockFactory.CreateLockAsync(
            resource: lockResource,
            expiryTime: LockTimeout,
            waitTime: TimeSpan.FromSeconds(5),
            retryTime: TimeSpan.FromMilliseconds(200),
            cancellationToken: cancellationToken
        );

        if (!redLock.IsAcquired)
            // Wait and retry once more, or return specific error
            throw new InvalidOperationException("Could not acquire lock - request with this idempotency key is already being processed.");

        // Double-check cache (lock acquired)
        cached = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<TResponse>(cached)!;

        // Execute
        var response = await next();

        // Store with status
        var record = new IdempotencyRecord<TResponse>
        {
            Status = IdempotencyStatus.Completed,
            Response = response,
            ExecutedAt = DateTime.UtcNow
        };

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(record),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = KeyTtl
            },
            cancellationToken
        );


        return response;
    }
}
