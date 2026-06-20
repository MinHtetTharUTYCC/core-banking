using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanPaymentMadeEvent : DomainEvent
{
    public Loan Loan { get; }
    public decimal Amount { get; }

    public LoanPaymentMadeEvent(Loan loan, decimal amount)
    {
        Loan = loan;
        Amount = amount;
    }
}
