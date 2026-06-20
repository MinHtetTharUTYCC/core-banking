using System.ComponentModel.DataAnnotations;
using CoreBanking.Domain.Enums;
using CoreBanking.Application.Loans.Queries;

namespace CoreBanking.Api.Models;

public class ApplyLoanRequest
{
    [Required]
    public Guid AccountId { get; init; }

    [Required]
    public LoanType LoanType { get; init; }

    [Required]
    [Range(0.01, 10_000_000, ErrorMessage = "Loan amount must be between 0.01 and 10,000,000")]
    public decimal PrincipalAmount { get; init; }

    [Required]
    public Currency Currency { get; init; }

    [Required]
    [Range(0, 100, ErrorMessage = "Interest rate must be between 0 and 100")]
    public decimal InterestRate { get; init; }

    [Required]
    [Range(1, 360, ErrorMessage = "Term must be between 1 and 360 months")]
    public int TermMonths { get; init; }
}

public class MakeLoanPaymentRequest
{
    [Required]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; init; }
}

public class LoanQueryParams
{
    public LoanSortOrder SortBy { get; init; } = LoanSortOrder.Newest;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
