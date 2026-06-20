using MediatR;

namespace CoreBanking.Application.Loans.Queries;

public record GetLoanByIdQuery : IRequest<LoanDto?>
{
    public Guid Id { get; init; }
}
