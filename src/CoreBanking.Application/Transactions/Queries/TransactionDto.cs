namespace CoreBanking.Application.Transactions.Queries;

public class TransactionDto
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal BalanceBefore { get; init; }
    public decimal BalanceAfter { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ReferenceNumber { get; init; }
    public Guid? RelatedAccountId { get; init; }
    public DateTime CreatedAt { get; init; }
}
