using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public class WithdrawCommandHandler(IAccountRepository repository) : IRequestHandler<WithdrawCommand, Unit>
{
    public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.AccountId) 
                      ?? throw new KeyNotFoundException("Account not found.");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        
        account.Withdraw(request.Amount);
        
        return Unit.Value;
    }
}
