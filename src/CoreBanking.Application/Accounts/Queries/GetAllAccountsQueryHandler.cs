using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Queries;

public class GetAllAccountsQueryHandler(IAccountRepository repository) : IRequestHandler<GetAllAccountsQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await repository.GetAllByEmailAsync(request.OwnerEmail);
        
        return accounts.Select(a => new AccountDto
        {
            Id = a.Id,
            AccountNumber = a.AccountNumber.Value,
            OwnerName = a.OwnerName,
            OwnerEmail = a.OwnerEmail,
            AccountType = a.AccountType.ToString(),
            Balance = a.Balance.Amount,
            Currency = a.Balance.Currency.ToString(),
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        }).ToList();
    }
}
