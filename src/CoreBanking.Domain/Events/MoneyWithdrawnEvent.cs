using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class MoneyWithdrawnEvent : DomainEvent
{
    public Account Account { get; }
    public decimal Amount { get; }
    
    public MoneyWithdrawnEvent(Account account, decimal amount)
    {
        Account = account;
        Amount = amount;
    }
}
