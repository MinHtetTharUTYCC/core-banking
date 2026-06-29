using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Loans.Queries;

public class GetLoansByAccountIdQueryHandler(ILoanRepository repository) : IRequestHandler<GetLoansByAccountIdQuery, PaginatedResult<LoanDto>>
{
    public async Task<PaginatedResult<LoanDto>> Handle(GetLoansByAccountIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByAccountIdAsync(request.AccountId, request.SortBy, request.Page, request.PageSize);

        return new PaginatedResult<LoanDto>
        {
            Items = result.Items.Select(GetAllLoansQueryHandler.MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }
}
