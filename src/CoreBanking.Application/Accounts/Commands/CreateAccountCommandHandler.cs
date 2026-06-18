using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _repository;
    
    public CreateAccountCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = Domain.Entities.Account.Create(
            request.OwnerName,
            request.OwnerEmail,
            request.AccountType,
            request.Currency);
        
        await _repository.AddAsync(account, cancellationToken);
        return account.Id;
    }
}
