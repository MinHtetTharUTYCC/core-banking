using MediatR;
using CoreBanking.Application.Accounts.Commands;

namespace CoreBanking.Application.Accounts.Commands;

public record DepositCommand : IRequest<Unit>
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string OwnerEmail { get; init; } = string.Empty;
}
