namespace CoreBanking.Domain.ValueObjects;

public record AccountNumber
{
    public string Value { get; }

    public AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Account number cannot be empty");
        
        if (value.Length < 10 || value.Length > 12)
            throw new ArgumentException("Account number must be 10-12 digits");
        
        if (!value.All(char.IsDigit))
            throw new ArgumentException("Account number must contain only digits");
        
        Value = value;
    }

    public static AccountNumber Generate()
    {
        var random = new Random();
        var number = string.Concat(Enumerable.Range(0, 12).Select(_ => random.Next(10).ToString()));
        return new AccountNumber(number);
    }
}
