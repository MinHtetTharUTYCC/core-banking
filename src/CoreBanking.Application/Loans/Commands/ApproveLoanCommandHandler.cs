using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class ApproveLoanCommandHandler : IRequestHandler<ApproveLoanCommand, Unit>
{
    private readonly ILoanRepository _repository;

    public ApproveLoanCommandHandler(ILoanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _repository.GetByIdAsync(request.LoanId);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found.");

        loan.Approve();
        await _repository.UpdateAsync(loan, cancellationToken);
        return Unit.Value;
    }
}
