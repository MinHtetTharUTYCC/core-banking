using FluentValidation;

namespace CoreBanking.Application.Loans.Commands;

public class MakeLoanPaymentCommandValidator : AbstractValidator<MakeLoanPaymentCommand>
{
    public MakeLoanPaymentCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be positive");
    }
}
