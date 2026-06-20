using MediatR;
using CoreBanking.Domain.Enums;

namespace CoreBanking.Application.Loans.Commands;

public record ApplyLoanCommand : IRequest<Guid>
{
    public Guid AccountId { get; init; }
    public LoanType LoanType { get; init; }
    public decimal PrincipalAmount { get; init; }
    public Currency Currency { get; init; }
    public decimal InterestRate { get; init; }
    public int TermMonths { get; init; }
}
