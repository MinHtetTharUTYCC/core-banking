using FluentValidation;

namespace CoreBanking.Application.Loans.Queries;

public class GetLoanByIdQueryValidator : AbstractValidator<GetLoanByIdQuery>
{
    public GetLoanByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Loan ID is required");
    }
}
