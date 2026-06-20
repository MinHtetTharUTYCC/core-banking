using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Loans.Queries;

public class GetLoansByAccountIdQueryHandler : IRequestHandler<GetLoansByAccountIdQuery, PaginatedResult<LoanDto>>
{
    private readonly ILoanRepository _repository;

    public GetLoansByAccountIdQueryHandler(ILoanRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<LoanDto>> Handle(GetLoansByAccountIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByAccountIdAsync(request.AccountId, request.SortBy, request.Page, request.PageSize);

        return new PaginatedResult<LoanDto>
        {
            Items = result.Items.Select(GetAllLoansQueryHandler.MapToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }
}
