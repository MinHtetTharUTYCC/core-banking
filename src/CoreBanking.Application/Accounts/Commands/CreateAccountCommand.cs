using MediatR;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public record CreateAccountCommand : IRequest<Guid>
{
    public string OwnerName { get; init; } = string.Empty;
    public string OwnerEmail { get; init; } = string.Empty;
    public AccountType AccountType { get; init; }
    public Currency Currency { get; init; }
}
