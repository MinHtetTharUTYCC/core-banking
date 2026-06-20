using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanAppliedEvent : DomainEvent
{
    public Loan Loan { get; }

    public LoanAppliedEvent(Loan loan)
    {
        Loan = loan;
    }
}
