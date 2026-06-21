using MediatR;

namespace CoreBanking.Application.Accounts.Commands;

public record TransferCommand : IRequest<Unit>
{
    public Guid FromAccountId { get; init; }
    public Guid ToAccountId { get; init; }
    public decimal Amount { get; init; }
    public string OwnerEmail { get; init; } = string.Empty;
}
