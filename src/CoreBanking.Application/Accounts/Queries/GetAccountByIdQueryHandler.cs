using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Queries;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IAccountRepository _repository;
    
    public GetAccountByIdQueryHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.Id);
        if (account == null)
            return null;
        
        return new AccountDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber.Value,
            OwnerName = account.OwnerName,
            OwnerEmail = account.OwnerEmail,
            AccountType = account.AccountType.ToString(),
            Balance = account.Balance.Amount,
            Currency = account.Balance.Currency.ToString(),
            Status = account.Status.ToString(),
            CreatedAt = account.CreatedAt
        };
    }
}
