using FluentValidation;

namespace CoreBanking.Application.Accounts.Commands;

public class TransferCommandValidator : AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(x => x.FromAccountId)
            .NotEmpty().WithMessage("Source account ID is required");

        RuleFor(x => x.ToAccountId)
            .NotEmpty().WithMessage("Destination account ID is required");

        RuleFor(x => x.FromAccountId)
            .NotEqual(x => x.ToAccountId).WithMessage("Cannot transfer to the same account");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfer amount must be positive");

        RuleFor(x => x.OwnerEmail)
            .NotEmpty().WithMessage("Owner email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("Idempotency key is required");
    }
}
