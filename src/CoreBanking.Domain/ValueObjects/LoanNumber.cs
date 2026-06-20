namespace CoreBanking.Domain.ValueObjects;

public record LoanNumber
{
    public string Value { get; }

    public LoanNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Loan number cannot be empty");

        if (value.Length < 10 || value.Length > 12)
            throw new ArgumentException("Loan number must be 10-12 characters");

        Value = value;
    }

    public static LoanNumber Generate()
    {
        var random = new Random();
        var number = "LN" + string.Concat(Enumerable.Range(0, 8).Select(_ => random.Next(10).ToString()));
        return new LoanNumber(number);
    }
}
