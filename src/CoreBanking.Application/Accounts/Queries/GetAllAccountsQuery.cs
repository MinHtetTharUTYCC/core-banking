using MediatR;

namespace CoreBanking.Application.Accounts.Queries;

public record GetAllAccountsQuery : IRequest<List<AccountDto>>;
