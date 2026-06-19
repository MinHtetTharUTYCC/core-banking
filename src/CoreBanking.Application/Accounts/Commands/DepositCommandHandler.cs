using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class DepositCommandHandler : IRequestHandler<DepositCommand, bool>
{
    private readonly IAccountRepository _repository;
    
    public DepositCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);
        if (account == null)
            return false;
        
        account.Deposit(request.Amount);
        await _repository.UpdateAsync(account);
        return true;
    }
}

public class DepositCommandHandlerCopy: IRequestHandler<DepositCommand,bool>
{
    private readonly IAccountRepository _repository;

    public DepositCommandHandlerCopy(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DepositCommand request,CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);

        if(account == null) return false;

        account.Deposit(request.Amount);
        await _repository.UpdateAsync(account);
        return true;
    }
}
