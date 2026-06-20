using MediatR;

namespace CoreBanking.Application.Loans.Commands;

public record ApproveLoanCommand : IRequest<bool>
{
    public Guid LoanId { get; init; }
}
