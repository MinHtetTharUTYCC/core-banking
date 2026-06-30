using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class MoneyDepositedEvent(Account account, decimal amount) : DomainEvent
{
    public Account Account { get; } = account;
    public decimal Amount { get; } = amount;
}
