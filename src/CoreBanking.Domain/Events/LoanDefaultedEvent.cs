using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanDefaultedEvent(Loan loan) : DomainEvent
{
    public Loan Loan { get; } = loan;
}