using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class MoneyTransferredEvent(Account fromAccount, Account toAccount, decimal amount) : DomainEvent
{
    public Account FromAccount { get; } = fromAccount;
    public Account ToAccount { get; } = toAccount;
    public decimal Amount { get; } = amount;
}
