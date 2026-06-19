using FluentValidation;

namespace CoreBanking.Application.Transactions.Queries;

public class GetAllTransactionsQueryValidator : AbstractValidator<GetAllTransactionsQuery>
{
    public GetAllTransactionsQueryValidator()
    {
        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage("Invalid sort order");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
    }
}
