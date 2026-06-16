using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class MoneyTransferredEvent : DomainEvent
{
    public Account FromAccount { get; }
    public Account ToAccount { get; }
    public decimal Amount { get; }
    
    public MoneyTransferredEvent(Account fromAccount, Account toAccount, decimal amount)
    {
        FromAccount = fromAccount;
        ToAccount = toAccount;
        Amount = amount;
    }
}
