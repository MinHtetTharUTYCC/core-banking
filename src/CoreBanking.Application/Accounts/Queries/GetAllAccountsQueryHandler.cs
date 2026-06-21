using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Queries;

public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, List<AccountDto>>
{
    private readonly IAccountRepository _repository;
    
    public GetAllAccountsQueryHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<AccountDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _repository.GetAllByEmailAsync(request.OwnerEmail);
        
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
