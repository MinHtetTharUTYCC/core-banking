using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanDefaultedEvent: DomainEvent
{
    public Loan Loan { get; }

    public LoanDefaultedEvent(Loan loan)
    {
        Loan = loan;
    }
}