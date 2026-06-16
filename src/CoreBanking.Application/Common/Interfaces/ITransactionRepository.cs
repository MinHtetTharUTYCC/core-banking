using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<List<Transaction>> GetByAccountIdAsync(Guid accountId);
    Task<List<Transaction>> GetAllAsync();
    Task AddAsync(Transaction transaction);
}
