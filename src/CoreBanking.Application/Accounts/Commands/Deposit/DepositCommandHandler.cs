using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands.Deposit;

public class DepositCommandHandler(IAccountRepository repository) : IRequestHandler<DepositCommand, Unit>
{

    public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.AccountId)
                      ?? throw new KeyNotFoundException("Account not found");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new UnauthorizedAccessException("You do not own this account");
        
        account.Deposit(request.Amount);
        
        return Unit.Value;
    }
}
