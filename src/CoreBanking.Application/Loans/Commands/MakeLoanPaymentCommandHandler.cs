using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class MakeLoanPaymentCommandHandler(ILoanRepository repository) : IRequestHandler<MakeLoanPaymentCommand, Unit>
{
    public async Task<Unit> Handle(MakeLoanPaymentCommand request, CancellationToken cancellationToken)
    {
        var loan = await repository.GetByIdAsync(request.LoanId);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found");

        loan.MakePayment(request.Amount);
        await repository.UpdateAsync(loan, cancellationToken);

        return Unit.Value;
    }
}
