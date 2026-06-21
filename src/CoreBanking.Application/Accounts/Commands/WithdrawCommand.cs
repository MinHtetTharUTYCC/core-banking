using MediatR;

namespace CoreBanking.Application.Accounts.Commands;

public record WithdrawCommand : IRequest<Unit>
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string OwnerEmail { get; init; } = string.Empty;
}