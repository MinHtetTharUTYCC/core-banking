using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class AccountCreatedEvent : DomainEvent
{
    public Account Account { get; }
    
    public AccountCreatedEvent(Account account)
    {
        Account = account;
    }
}
