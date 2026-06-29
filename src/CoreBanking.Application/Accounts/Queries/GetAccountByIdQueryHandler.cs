using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Exceptions;

namespace CoreBanking.Application.Accounts.Queries;

public class GetAccountByIdQueryHandler(IAccountRepository repository) : IRequestHandler<GetAccountByIdQuery, AccountDto>
{
    public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.Id);

        if (account == null)
            throw new KeyNotFoundException("Account not found.");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new BadRequestException("Unauthorized access to account.");

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
