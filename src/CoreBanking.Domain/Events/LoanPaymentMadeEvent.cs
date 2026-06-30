using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanPaymentMadeEvent(Loan loan, decimal amount) : DomainEvent
{
    public Loan Loan { get; } = loan;
    public decimal Amount { get; } = amount;
}
