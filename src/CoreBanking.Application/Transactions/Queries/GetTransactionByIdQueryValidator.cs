using FluentValidation;

namespace CoreBanking.Application.Transactions.Queries;

public class GetTransactionByIdQueryValidator : AbstractValidator<GetTransactionByIdQuery>
{
    public GetTransactionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Transaction ID is required");
    }
}
