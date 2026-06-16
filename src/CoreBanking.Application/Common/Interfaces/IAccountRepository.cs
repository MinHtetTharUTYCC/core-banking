using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Common.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetByAccountNumberAsync(string accountNumber);
    Task<List<Account>> GetAllAsync();
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
}
