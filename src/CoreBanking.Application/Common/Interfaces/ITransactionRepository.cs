using CoreBanking.Application.Common.Models;
using CoreBanking.Application.Transactions.Queries;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<PaginatedResult<Transaction>> GetAllAsync(TransactionSortOrder sortBy, int page, int pageSize);
    Task<PaginatedResult<Transaction>> GetByAccountIdAsync(Guid accountId, TransactionSortOrder sortBy, int page, int pageSize);
    Task AddAsync(Transaction transaction);
}
