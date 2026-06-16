using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Accounts.Commands;

public class TransferCommandHandler : IRequestHandler<TransferCommand, bool>
{
    private readonly IAccountRepository _repository;
    
    public TransferCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var fromAccount = await _repository.GetByIdAsync(request.FromAccountId);
        var toAccount = await _repository.GetByIdAsync(request.ToAccountId);
        
        if (fromAccount == null || toAccount == null)
            return false;
        
        fromAccount.Transfer(toAccount, request.Amount);
        
        await _repository.UpdateAsync(fromAccount);
        await _repository.UpdateAsync(toAccount);
        return true;
    }
}
