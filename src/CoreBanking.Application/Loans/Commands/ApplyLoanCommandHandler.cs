using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Commands;

public class ApplyLoanCommandHandler : IRequestHandler<ApplyLoanCommand, Guid>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;

    public ApplyLoanCommandHandler(ILoanRepository loanRepository, IAccountRepository accountRepository)
    {
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Guid> Handle(ApplyLoanCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null)
            throw new InvalidOperationException("Account not found");

        var loan = Domain.Entities.Loan.Apply(
            account,
            request.LoanType,
            request.PrincipalAmount,
            request.Currency,
            request.InterestRate,
            request.TermMonths);

        await _loanRepository.AddAsync(loan, cancellationToken);
        return loan.Id;
    }
}
