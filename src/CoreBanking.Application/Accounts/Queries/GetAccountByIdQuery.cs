using MediatR;

namespace CoreBanking.Application.Accounts.Queries;

public record GetAccountByIdQuery : IRequest<AccountDto>
{
    public Guid Id { get; init; }
    public string OwnerEmail { get; init; } = string.Empty;
}

public class AccountDto
{
    public Guid Id { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public string OwnerName { get; init; } = string.Empty;
    public string OwnerEmail { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
