using MediatR;

namespace CoreBanking.Application.Loans.Commands;

public record DisburseLoanCommand : IRequest<Unit>
{
    public Guid LoanId { get; init; }
}
