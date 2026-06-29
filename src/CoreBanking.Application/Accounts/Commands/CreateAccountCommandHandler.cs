using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class CreateAccountCommandHandler(IAccountRepository repository) : IRequestHandler<CreateAccountCommand, Guid>
{
 
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = Domain.Entities.Account.Create(
            request.OwnerName,
            request.OwnerEmail,
            request.AccountType,
            request.Currency);
        
        await repository.AddAsync(account, cancellationToken);
        return account.Id;
    }
}
