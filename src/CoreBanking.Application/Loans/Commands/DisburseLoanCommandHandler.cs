using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class DisburseLoanCommandHandler(ILoanRepository loanRepository,
    IAccountRepository accountRepository) : IRequestHandler<DisburseLoanCommand, Unit>
{
    public async Task<Unit> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await loanRepository.GetByIdAsync(request.LoanId);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found.");

        loan.Disburse();
        loan.Activate();

        var account = await accountRepository.GetByIdAsync(loan.AccountId);
        if (account != null)
        {
            account.Deposit(loan.PrincipalAmount.Amount);
            await accountRepository.UpdateAsync(account, cancellationToken);
        }

        await loanRepository.UpdateAsync(loan, cancellationToken);

        return Unit.Value;
    }
}
