using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Accounts.Commands;

public class DepositCommandHandler : IRequestHandler<DepositCommand, Unit>
{
    private readonly IAccountRepository _repository;
    private readonly ITransactionRepository _transactionRepository;

    public DepositCommandHandler(IAccountRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Unit> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);

        if (account is null)
            throw new KeyNotFoundException("Account not found");

        if (account.OwnerEmail != request.OwnerEmail)
            throw new UnauthorizedAccessException("You do not own this account");

        var balanceBefore = account.Balance.Amount;

        var transaction = Transaction.Create(
            account: account,
            type: TransactionType.Credit,
            amount: request.Amount,
            currency: account.Balance.Currency,
            balanceBefore: balanceBefore,
            description: "Deposit");

        await _transactionRepository.AddAsync(transaction,cancellationToken);

        try
        {
            transaction.StartProcessing();
            account.Deposit(request.Amount);
            transaction.Complete();
        }
        catch (Exception)
        {
            transaction.Fail();
            throw;
        }

        await _repository.UpdateAsync(account);

        return Unit.Value;
    }
}
