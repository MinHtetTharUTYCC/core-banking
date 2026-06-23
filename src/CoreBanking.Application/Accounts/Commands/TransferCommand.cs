using MediatR;
using CoreBanking.Application.Common.Behaviors;

namespace CoreBanking.Application.Accounts.Commands;

public record TransferCommand : IRequest<Unit>, IIdempotentRequest
{
    public Guid FromAccountId { get; init; }
    public Guid ToAccountId { get; init; }
    public decimal Amount { get; init; }
    public string OwnerEmail { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
}
