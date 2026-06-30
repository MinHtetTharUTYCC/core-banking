using CoreBanking.Domain.Common;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Domain.Events;

public class LoanDisbursedEvent(Loan loan) : DomainEvent
{
    public Loan Loan { get; } = loan;
}
