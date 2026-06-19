using MediatR;
using CoreBanking.Application.Common.Models;

namespace CoreBanking.Application.Transactions.Queries;

public record GetTransactionsByAccountIdQuery : IRequest<PaginatedResult<TransactionDto>>
{
    public Guid AccountId { get; init; }
    public TransactionSortOrder SortBy { get; init; } = TransactionSortOrder.Newest;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
