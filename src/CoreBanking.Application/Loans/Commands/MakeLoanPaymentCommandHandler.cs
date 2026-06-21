using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class MakeLoanPaymentCommandHandler : IRequestHandler<MakeLoanPaymentCommand, Unit>
{
    private readonly ILoanRepository _repository;

    public MakeLoanPaymentCommandHandler(ILoanRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(MakeLoanPaymentCommand request, CancellationToken cancellationToken)
    {
        var loan = await _repository.GetByIdAsync(request.LoanId);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found");

        loan.MakePayment(request.Amount);
        await _repository.UpdateAsync(loan, cancellationToken);

        return Unit.Value;
    }
}
