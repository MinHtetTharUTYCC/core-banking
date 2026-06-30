using CoreBanking.Domain.Common;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Events;
using CoreBanking.Domain.ValueObjects;

namespace CoreBanking.Domain.Entities;

public class Account : BaseEntity
{
    private readonly List<Transaction> _transactions = new();
    
    public AccountNumber AccountNumber { get; private set; } = null!;
    public string OwnerName { get; private set; } = string.Empty;
    public string OwnerEmail { get; private set; } = string.Empty;
    public AccountType AccountType { get; private set; }
    public Money Balance { get; private set; } = null!;
    public AccountStatus Status { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    
    private Account() { }
    
    public static Account Create(string ownerName, string ownerEmail, AccountType accountType, Currency currency)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            AccountNumber = AccountNumber.Generate(),
            OwnerName = ownerName,
            OwnerEmail = ownerEmail,
            AccountType = accountType,
            Balance = Money.Zero(currency),
            Status = AccountStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        
        account.AddDomainEvent(new AccountCreatedEvent(account));
        return account;
    }
    
    public void Activate()
    {
        if (Status != AccountStatus.Pending)
            throw new InvalidOperationException("Only pending accounts can be activated");
        
        Status = AccountStatus.Active;
        ActivatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deposit(decimal amount)
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException("Account must be active to deposit");
        
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive");
        
        Credit(amount);
        AddDomainEvent(new MoneyDepositedEvent(this, amount));
    }

    public void Withdraw(decimal amount)
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException("Account must be active to withdraw");
        
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive");
        
        var withdrawalMoney = new Money(amount, Balance.Currency);
        Balance = Balance.Subtract(withdrawalMoney);
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new MoneyWithdrawnEvent(this, amount));
    }

    public void Transfer(Account toAccount, decimal amount)
    {
        if (Status != AccountStatus.Active)
            throw new InvalidOperationException("Source account must be active");
        
        if (toAccount.Status != AccountStatus.Active)
            throw new InvalidOperationException("Destination account must be active");
        
        if (Balance.Currency != toAccount.Balance.Currency)
            throw new InvalidOperationException("Cannot transfer between different currencies");
        
        Withdraw(amount);
        toAccount.Credit(amount);
        
        AddDomainEvent(new MoneyTransferredEvent(this, toAccount, amount));
    }
    
    private void Credit(decimal amount)
    {
        var creditMoney = new Money(amount, Balance.Currency);
        Balance = Balance.Add(creditMoney);
        UpdatedAt = DateTime.UtcNow;
    }

}

public enum AccountStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Closed = 3
}
