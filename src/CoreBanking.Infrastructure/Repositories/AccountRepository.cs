using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;

namespace CoreBanking.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;
    
    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }
    
    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await _context.Accounts.FindAsync(id);
    }
    
    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber.Value == accountNumber);
    }
    
    public async Task<List<Account>> GetAllAsync()
    {
        return await _context.Accounts.ToListAsync();
    }
    
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
