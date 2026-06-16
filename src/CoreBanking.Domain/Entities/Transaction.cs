using CoreBanking.Domain.Common;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }
    public decimal BalanceBefore { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? Description { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public Guid? RelatedAccountId { get; private set; }
    
    public Account Account { get; private set; } = null!;
    public Account? RelatedAccount { get; private set; }
    
    private Transaction() { }
    
    public static Transaction Create(
        Account account,
        TransactionType type,
        decimal amount,
        Currency currency,
        decimal balanceBefore,
        string? description = null,
        Guid? relatedAccountId = null)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Type = type,
            Amount = amount,
            Currency = currency,
            BalanceBefore = balanceBefore,
            BalanceAfter = type == TransactionType.Credit 
                ? balanceBefore + amount 
                : balanceBefore - amount,
            Status = TransactionStatus.Pending,
            Description = description,
            ReferenceNumber = GenerateReferenceNumber(),
            RelatedAccountId = relatedAccountId,
            CreatedAt = DateTime.UtcNow
        };
        
        return transaction;
    }
    
    public void Complete()
    {
        Status = TransactionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Fail()
    {
        Status = TransactionStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Reverse()
    {
        Status = TransactionStatus.Reversed;
        UpdatedAt = DateTime.UtcNow;
    }
    
    private static string GenerateReferenceNumber()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}
