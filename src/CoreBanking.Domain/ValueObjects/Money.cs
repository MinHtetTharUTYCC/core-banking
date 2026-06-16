using CoreBanking.Domain.Enums;

namespace CoreBanking.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(Currency currency) => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract different currencies");
        
        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvalidOperationException("Insufficient funds");
        
        return new Money(result, Currency);
    }
}
