using System.Text.Json;
using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Common.Behaviors;

public interface IIdempotentRequest
{
    string IdempotencyKey { get; }
}

public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IIdempotentRequest
{
    private readonly IIdempotencyRepository _repository;

    public IdempotencyBehavior(IIdempotencyRepository repository)
    {
        _repository = repository;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var existingKey = await _repository.GetByKeyAsync(request.IdempotencyKey);

        if (existingKey is not null)
        {
            if (existingKey.Response is not null)
                return JsonSerializer.Deserialize<TResponse>(existingKey.Response)!;

            return await next();
        }

        var idempotencyKey = Domain.Entities.IdempotencyKey.Create(request.IdempotencyKey);
        await _repository.AddAsync(idempotencyKey);

        var response = await next();

        idempotencyKey.SetResponse(JsonSerializer.Serialize(response));
        await _repository.AddAsync(idempotencyKey);

        return response;
    }
}
