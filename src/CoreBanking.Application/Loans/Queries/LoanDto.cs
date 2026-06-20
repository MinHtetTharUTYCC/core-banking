namespace CoreBanking.Application.Loans.Queries;

public class LoanDto
{
    public Guid Id { get; init; }
    public string LoanNumber { get; init; } = string.Empty;
    public Guid AccountId { get; init; }
    public string LoanType { get; init; } = string.Empty;
    public decimal PrincipalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal OutstandingBalance { get; init; }
    public decimal InterestRate { get; init; }
    public int TermMonths { get; init; }
    public decimal MonthlyPayment { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime? DisbursedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public string? RejectionReason { get; init; }
}
