using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanApprovedEvent : DomainEvent
{
    public Loan Loan { get; }

    public LoanApprovedEvent(Loan loan)
    {
        Loan = loan;
    }
}
