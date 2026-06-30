using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class AccountCreatedEvent(Account account) : DomainEvent
{
    public Account Account { get; } = account;
}
