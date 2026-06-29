using MediatR;
using CoreBanking.Application.Common.Interfaces;

namespace CoreBanking.Application.Loans.Queries;

public class GetLoanByIdQueryHandler(ILoanRepository repository) : IRequestHandler<GetLoanByIdQuery, LoanDto>
{
    public async Task<LoanDto> Handle(GetLoanByIdQuery request, CancellationToken cancellationToken)
    {
        var loan = await repository.GetByIdAsync(request.Id);
        if (loan == null)
            throw new KeyNotFoundException("Loan not found");

        return new LoanDto
        {
            Id = loan.Id,
            LoanNumber = loan.LoanNumber.Value,
            AccountId = loan.AccountId,
            LoanType = loan.LoanType.ToString(),
            PrincipalAmount = loan.PrincipalAmount.Amount,
            Currency = loan.PrincipalAmount.Currency.ToString(),
            OutstandingBalance = loan.OutstandingBalance.Amount,
            InterestRate = loan.InterestRate,
            TermMonths = loan.TermMonths,
            MonthlyPayment = loan.MonthlyPayment,
            Status = loan.Status.ToString(),
            CreatedAt = loan.CreatedAt,
            ApprovedAt = loan.ApprovedAt,
            DisbursedAt = loan.DisbursedAt,
            ClosedAt = loan.ClosedAt,
            RejectionReason = loan.RejectionReason
        };
    }
}
