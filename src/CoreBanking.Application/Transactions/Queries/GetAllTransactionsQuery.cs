using MediatR;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Transactions.Queries;

public record GetAllTransactionsQuery : IRequest<PaginatedResult<TransactionDto>>
{
    public TransactionSortOrder SortBy { get; init; } = TransactionSortOrder.Newest;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
