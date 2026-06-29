using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class ApproveLoanCommandHandler(ILoanRepository repository) : IRequestHandler<ApproveLoanCommand, Unit>
{
    public async Task<Unit> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await repository.GetByIdAsync(request.LoanId);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found.");

        loan.Approve();
        await repository.UpdateAsync(loan, cancellationToken);
        return Unit.Value;
    }
}
