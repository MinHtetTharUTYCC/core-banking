using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Transactions.Queries;

public class GetTransactionsByAccountIdQueryHandler(ITransactionRepository repository) : IRequestHandler<GetTransactionsByAccountIdQuery, PaginatedResult<TransactionDto>>
{
    public async Task<PaginatedResult<TransactionDto>> Handle(GetTransactionsByAccountIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByAccountIdAsync(request.AccountId, request.SortBy, request.Page, request.PageSize);

        return new PaginatedResult<TransactionDto>
        {
            Items = result.Items.Select(t => GetTransactionByIdQueryHandler.MapToDto(t)).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }
}
