using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, bool>
{
    private readonly IAccountRepository _repository;
    
    public WithdrawCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);
        if (account == null)
            return false;
        
        account.Withdraw(request.Amount);
        await _repository.UpdateAsync(account);
        return true;
    }
}

public class WithdrawCommandHandlerCopy : IRequestHandler<WithdrawCommand, bool>
{
    private readonly IAccountRepository _repository;

    public WithdrawCommandHandlerCopy(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);
        if (account == null)
            return false;

        account.Withdraw(request.Amount);
        await _repository.UpdateAsync(account);
        return true;
    }
}
