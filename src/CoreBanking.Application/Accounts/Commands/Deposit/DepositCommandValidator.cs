using FluentValidation;

namespace CoreBanking.Application.Accounts.Commands;

public class DepositCommandValidator : AbstractValidator<DepositCommand>
{
    public DepositCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Deposit amount must be positive");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty().WithMessage("Owner email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required");
    }
}
