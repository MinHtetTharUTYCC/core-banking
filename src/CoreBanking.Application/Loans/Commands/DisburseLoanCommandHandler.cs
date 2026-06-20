using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class DisburseLoanCommandHandler : IRequestHandler<DisburseLoanCommand, bool>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;

    public DisburseLoanCommandHandler(ILoanRepository loanRepository, IAccountRepository accountRepository)
    {
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
    }

    public async Task<bool> Handle(DisburseLoanCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId);
        if (loan == null)
            return false;

        loan.Disburse();
        loan.Activate();

        var account = await _accountRepository.GetByIdAsync(loan.AccountId);
        if (account != null)
        {
            account.Deposit(loan.PrincipalAmount.Amount);
            await _accountRepository.UpdateAsync(account, cancellationToken);
        }

        await _loanRepository.UpdateAsync(loan, cancellationToken);
        return true;
    }
}
