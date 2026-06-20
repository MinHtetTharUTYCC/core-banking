using MediatR;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Loans.Queries;

public record GetAllLoansQuery : IRequest<PaginatedResult<LoanDto>>
{
    public LoanSortOrder SortBy { get; init; } = LoanSortOrder.Newest;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
