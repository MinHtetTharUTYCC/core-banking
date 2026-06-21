using MediatR;

namespace CoreBanking.Application.Loans.Commands;

public record ApproveLoanCommand : IRequest<Unit>
{
    public Guid LoanId { get; init; }
}
