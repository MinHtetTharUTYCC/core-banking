using MediatR;
using CoreBanking.Application.Common.Behaviors;

namespace CoreBanking.Application.Accounts.Commands;

public record DepositCommand : IRequest<Unit>, IIdempotentRequest
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string OwnerEmail { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
}
