using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public class WithdrawCommandHandler(IAccountRepository repository,
    ITransactionRepository transactionRepository) : IRequestHandler<WithdrawCommand, Unit>
{
    public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await repository.GetByIdAsync(request.AccountId);

        if (account is null)
            throw new KeyNotFoundException("Account not found.");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");

        var balanceBefore = account.Balance.Amount;

        var transaction = Transaction.Create(
            account: account,
            type: TransactionType.Debit,
            amount: request.Amount,
            currency: account.Balance.Currency,
            balanceBefore: balanceBefore,
            description: "Withdrawal");

        await transactionRepository.AddAsync(transaction,cancellationToken);

        try
        {
            transaction.StartProcessing();
            account.Withdraw(request.Amount);
            transaction.Complete();
        }
        catch (Exception)
        {
            transaction.Fail();
            throw;
        }

        await repository.UpdateAsync(account);

        return Unit.Value;
    }
}
