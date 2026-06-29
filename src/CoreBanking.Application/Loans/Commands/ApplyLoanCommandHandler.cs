using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class ApplyLoanCommandHandler(ILoanRepository loanRepository,
    IAccountRepository accountRepository) : IRequestHandler<ApplyLoanCommand, Guid>
{
    public async Task<Guid> Handle(ApplyLoanCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        var loan = Domain.Entities.Loan.Apply(
            account,
            request.LoanType,
            request.PrincipalAmount,
            request.Currency,
            request.InterestRate,
            request.TermMonths);

        await loanRepository.AddAsync(loan, cancellationToken);
        return loan.Id;
    }
}
