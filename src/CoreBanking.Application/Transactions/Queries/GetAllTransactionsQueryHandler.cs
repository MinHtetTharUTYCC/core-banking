using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Transactions.Queries;

public class GetAllTransactionsQueryHandler(ITransactionRepository repository) : IRequestHandler<GetAllTransactionsQuery, PaginatedResult<TransactionDto>>
{

    public async Task<PaginatedResult<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetAllAsync(request.SortBy, request.Page, request.PageSize);

        return new PaginatedResult<TransactionDto>
        {
            Items = result.Items.Select(t => GetTransactionByIdQueryHandler.MapToDto(t)).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }
}
