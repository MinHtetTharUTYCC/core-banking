using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;

namespace CoreBanking.Infrastructure.Repositories;

public class AccountRepository(BankingDbContext context) : IAccountRepository
{
    
    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await context.Accounts.FindAsync(id);
    }
    
    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber.Value == accountNumber);
    }
    
    public async Task<List<Account>> GetAllAsync()
    {
        return await context.Accounts.ToListAsync();
    }

    public async Task<List<Account>> GetAllByEmailAsync(string email)
    {
        return await context.Accounts
            .Where(a => a.OwnerEmail == email)
            .ToListAsync();
    }
    
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await context.Accounts.AddAsync(account, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        context.Accounts.Update(account);
        await context.SaveChangesAsync(cancellationToken);
    }
}
