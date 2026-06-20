using MediatR;

namespace CoreBanking.Application.Loans.Commands;

public record MakeLoanPaymentCommand : IRequest<bool>
{
    public Guid LoanId { get; init; }
    public decimal Amount { get; init; }
}
