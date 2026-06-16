using MediatR;

namespace CoreBanking.Application.Accounts.Commands;

public record DepositCommand : IRequest<bool>
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
}
