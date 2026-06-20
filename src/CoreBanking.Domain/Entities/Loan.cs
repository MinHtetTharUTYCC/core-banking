using CoreBanking.Domain.Common;
using CoreBanking.Domain.Enums;
using CoreBanking.Domain.Events;
using CoreBanking.Domain.ValueObjects;

namespace CoreBanking.Domain.Entities;

public class Loan : BaseEntity
{
    public LoanNumber LoanNumber { get; private set; } = null!;
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;
    public LoanType LoanType { get; private set; }
    public Money PrincipalAmount { get; private set; } = null!;
    public Money OutstandingBalance { get; private set; } = null!;
    public decimal InterestRate { get; private set; }
    public int TermMonths { get; private set; }
    public decimal MonthlyPayment { get; private set; }
    public LoanStatus Status { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? DisbursedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    private Loan() { }

    public static Loan Apply(
        Account account,
        LoanType loanType,
        decimal principalAmount,
        Currency currency,
        decimal interestRate,
        int termMonths)
    {
        if (account.Status != AccountStatus.Active)
            throw new InvalidOperationException("Account must be active to apply for a loan");

        if (principalAmount <= 0)
            throw new ArgumentException("Loan amount must be positive");

        if (interestRate < 0 || interestRate > 100)
            throw new ArgumentException("Interest rate must be between 0 and 100");

        if (termMonths <= 0)
            throw new ArgumentException("Loan term must be positive");

        var principal = new Money(principalAmount, currency);
        var monthlyPayment = CalculateMonthlyPayment(principalAmount, interestRate, termMonths);

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            LoanNumber = LoanNumber.Generate(),
            AccountId = account.Id,
            Account = account,
            LoanType = loanType,
            PrincipalAmount = principal,
            OutstandingBalance = principal,
            InterestRate = interestRate,
            TermMonths = termMonths,
            MonthlyPayment = monthlyPayment,
            Status = LoanStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        loan.AddDomainEvent(new LoanAppliedEvent(loan));
        return loan;
    }

    public void Approve()
    {
        if (Status != LoanStatus.Pending)
            throw new InvalidOperationException("Only pending loans can be approved");

        Status = LoanStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LoanApprovedEvent(this));
    }

    public void Reject(string reason)
    {
        if (Status != LoanStatus.Pending)
            throw new InvalidOperationException("Only pending loans can be rejected");

        Status = LoanStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Disburse()
    {
        if (Status != LoanStatus.Approved)
            throw new InvalidOperationException("Only approved loans can be disbursed");

        Status = LoanStatus.Disbursed;
        DisbursedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LoanDisbursedEvent(this));
    }

    public void Activate()
    {
        if (Status != LoanStatus.Disbursed)
            throw new InvalidOperationException("Only disbursed loans can be activated");

        Status = LoanStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MakePayment(decimal amount)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("Can only make payments on active loans");

        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        if (amount > OutstandingBalance.Amount)
            throw new ArgumentException("Payment amount exceeds outstanding balance");

        var paymentMoney = new Money(amount, OutstandingBalance.Currency);
        OutstandingBalance = OutstandingBalance.Subtract(paymentMoney);
        UpdatedAt = DateTime.UtcNow;

        if (OutstandingBalance.Amount == 0)
        {
            Status = LoanStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }

        AddDomainEvent(new LoanPaymentMadeEvent(this, amount));
    }

    public void MarkDefaulted()
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("Only active loans can be marked as defaulted");

        Status = LoanStatus.Defaulted;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new LoanDefaultedEvent(this));
    }

    private static decimal CalculateMonthlyPayment(decimal principal, decimal annualRate, int termMonths)
    {
        if (annualRate == 0)
            return principal / termMonths;

        var monthlyRate = annualRate / 100 / 12;
        var factor = (decimal)Math.Pow(1 + (double)monthlyRate, termMonths);
        return principal * monthlyRate * factor / (factor - 1);
    }
}
