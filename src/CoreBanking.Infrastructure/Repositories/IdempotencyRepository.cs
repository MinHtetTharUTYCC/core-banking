using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;

namespace CoreBanking.Infrastructure.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly BankingDbContext _context;

    public IdempotencyRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<IdempotencyKey?> GetByKeyAsync(string key)
    {
        return await _context.IdempotencyKeys
            .FirstOrDefaultAsync(k => k.Key == key && !k.IsExpired);
    }

    public async Task AddAsync(IdempotencyKey idempotencyKey)
    {
        _context.IdempotencyKeys.Add(idempotencyKey);
        await _context.SaveChangesAsync();
    }
}
