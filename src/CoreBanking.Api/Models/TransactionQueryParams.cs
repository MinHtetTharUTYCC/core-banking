using System.ComponentModel.DataAnnotations;
using CoreBanking.Application.Transactions.Queries;

namespace CoreBanking.Api.Models;

public class TransactionQueryParams
{
    public TransactionSortOrder SortBy { get; set; } = TransactionSortOrder.Newest;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}