using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Exceptions;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Unit>
{
    private readonly IAccountRepository _repository;
    private readonly ITransactionRepository _transactionRepository;

    public TransferCommandHandler(IAccountRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var fromAccount = await _repository.GetByIdAsync(request.FromAccountId);

        if (fromAccount == null)
            throw new KeyNotFoundException("Account not found");

        if (fromAccount.OwnerEmail != request.OwnerEmail)
            throw new BadRequestException("You are not authorized to transfer from this account");

        var toAccount = await _repository.GetByIdAsync(request.ToAccountId);
        if (toAccount == null)
            throw new KeyNotFoundException("Account to transfer to not found");

        var fromBalanceBefore = fromAccount.Balance.Amount;
        var toBalanceBefore = toAccount.Balance.Amount;

        var debitTransaction = Transaction.Create(
            account: fromAccount,
            type: TransactionType.Debit,
            amount: request.Amount,
            currency: fromAccount.Balance.Currency,
            balanceBefore: fromBalanceBefore,
            description: $"Transfer to {toAccount.AccountNumber.Value}",
            relatedAccountId: toAccount.Id);

        var creditTransaction = Transaction.Create(
            account: toAccount,
            type: TransactionType.Credit,
            amount: request.Amount,
            currency: toAccount.Balance.Currency,
            balanceBefore: toBalanceBefore,
            description: $"Transfer from {fromAccount.AccountNumber.Value}",
            relatedAccountId: fromAccount.Id);

        await _transactionRepository.AddAsync(debitTransaction, cancellationToken);
        await _transactionRepository.AddAsync(creditTransaction, cancellationToken);

        try
        {
            debitTransaction.StartProcessing();
            creditTransaction.StartProcessing();

            fromAccount.Transfer(toAccount, request.Amount);

            debitTransaction.Complete();
            creditTransaction.Complete();
        }
        catch (Exception)
        {
            debitTransaction.Fail();
            creditTransaction.Fail();
            throw;
        }

        await _repository.UpdateAsync(fromAccount);
        await _repository.UpdateAsync(toAccount);

        return Unit.Value;
    }
}
