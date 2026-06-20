using FluentValidation;

namespace CoreBanking.Application.Loans.Commands;

public class ApplyLoanCommandValidator : AbstractValidator<ApplyLoanCommand>
{
    public ApplyLoanCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.PrincipalAmount)
            .GreaterThan(0).WithMessage("Loan amount must be positive")
            .LessThanOrEqualTo(10_000_000).WithMessage("Loan amount cannot exceed 10,000,000");

        RuleFor(x => x.InterestRate)
            .InclusiveBetween(0, 100).WithMessage("Interest rate must be between 0 and 100");

        RuleFor(x => x.TermMonths)
            .InclusiveBetween(1, 360).WithMessage("Term must be between 1 and 360 months");
    }
}
