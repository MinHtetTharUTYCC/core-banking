using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Unit>
{
    private readonly IAccountRepository _repository;
    private readonly ITransactionRepository _transactionRepository;

    public WithdrawCommandHandler(IAccountRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);

        if (account is null)
            throw new KeyNotFoundException("Account not found.");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");

        var balanceBefore = account.Balance.Amount;

        account.Withdraw(request.Amount);

        var transaction = Transaction.Create(
            account: account,
            type: TransactionType.Debit,
            amount: request.Amount,
            currency: account.Balance.Currency,
            balanceBefore: balanceBefore,
            description: "Withdrawal");
        transaction.Complete();

        await _transactionRepository.AddAsync(transaction);
        await _repository.UpdateAsync(account);

        return Unit.Value;
    }
}
