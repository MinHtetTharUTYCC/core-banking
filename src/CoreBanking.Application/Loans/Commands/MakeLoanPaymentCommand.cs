using MediatR;

namespace CoreBanking.Application.Loans.Commands;

public record MakeLoanPaymentCommand : IRequest<Unit>
{
    public Guid LoanId { get; init; }
    public decimal Amount { get; init; }
}
