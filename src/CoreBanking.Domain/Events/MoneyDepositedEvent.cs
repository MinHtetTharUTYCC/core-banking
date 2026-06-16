using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class MoneyDepositedEvent : DomainEvent
{
    public Account Account { get; }
    public decimal Amount { get; }
    
    public MoneyDepositedEvent(Account account, decimal amount)
    {
        Account = account;
        Amount = amount;
    }
}
