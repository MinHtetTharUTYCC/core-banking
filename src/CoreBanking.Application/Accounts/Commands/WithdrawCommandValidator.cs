using FluentValidation;

namespace CoreBanking.Application.Accounts.Commands;

public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Withdrawal amount must be positive");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty().WithMessage("Owner email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required");
    }
}
