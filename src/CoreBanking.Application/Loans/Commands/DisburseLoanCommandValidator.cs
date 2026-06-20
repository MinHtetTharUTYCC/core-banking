using FluentValidation;

namespace CoreBanking.Application.Loans.Commands;

public class DisburseLoanCommandValidator : AbstractValidator<DisburseLoanCommand>
{
    public DisburseLoanCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required");
    }
}
