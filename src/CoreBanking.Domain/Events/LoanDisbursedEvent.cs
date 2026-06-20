using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanDisbursedEvent : DomainEvent
{
    public Loan Loan { get; }

    public LoanDisbursedEvent(Loan loan)
    {
        Loan = loan;
    }
}
