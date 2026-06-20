using MediatR;

namespace CoreBanking.Application.Loans.Commands;

public record DisburseLoanCommand : IRequest<bool>
{
    public Guid LoanId { get; init; }
}
