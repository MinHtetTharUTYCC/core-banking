using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class DisburseLoanCommandHandler : IRequestHandler<DisburseLoanCommand, Unit>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;

    public DisburseLoanCommandHandler(ILoanRepository loanRepository, IAccountRepository accountRepository)
    {
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Unit> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found.");

        loan.Disburse();
        loan.Activate();

        var account = await _accountRepository.GetByIdAsync(loan.AccountId);
        if (account != null)
        {
            account.Deposit(loan.PrincipalAmount.Amount);
            await _accountRepository.UpdateAsync(account, cancellationToken);
        }

        await _loanRepository.UpdateAsync(loan, cancellationToken);

        return Unit.Value;
    }
}
