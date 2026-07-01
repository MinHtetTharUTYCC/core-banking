using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Exceptions;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public class TransferCommandHandler(IAccountRepository repository) : IRequestHandler<TransferCommand, Unit>
{

    public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var fromAccount = await repository.GetByIdAsync(request.FromAccountId) 
                          ?? throw new KeyNotFoundException("Account not found");

        if (fromAccount.OwnerEmail != request.OwnerEmail)
            throw new BadRequestException("You are not authorized to transfer from this account");

        var toAccount = await repository.GetByIdAsync(request.ToAccountId) 
                        ?? throw new KeyNotFoundException("Account to transfer to not found");

        fromAccount.Transfer(toAccount, request.Amount);
        
        await repository.UpdateAsync(fromAccount, cancellationToken); 
        await repository.UpdateAsync(toAccount, cancellationToken);

        return Unit.Value;
    }
}
