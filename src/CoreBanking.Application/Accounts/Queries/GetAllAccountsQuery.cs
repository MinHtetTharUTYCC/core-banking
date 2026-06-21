using MediatR;

namespace CoreBanking.Application.Accounts.Queries;

public record GetAllAccountsQuery : IRequest<List<AccountDto>>
{
    public string OwnerEmail { get; init; } = string.Empty;
}
