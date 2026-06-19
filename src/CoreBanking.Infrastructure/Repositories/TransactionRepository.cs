using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;
using CoreBanking.Application.Transactions.Queries;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;

namespace CoreBanking.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingDbContext _context;

    public TransactionRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    public async Task<PaginatedResult<Transaction>> GetAllAsync(TransactionSortOrder sortBy, int page, int pageSize)
    {
        var query = _context.Transactions.AsQueryable();
        query = ApplySorting(query, sortBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Transaction>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<Transaction>> GetByAccountIdAsync(Guid accountId, TransactionSortOrder sortBy, int page, int pageSize)
    {
        var query = _context.Transactions
            .Where(t => t.AccountId == accountId);
        query = ApplySorting(query, sortBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Transaction>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    private static IQueryable<Transaction> ApplySorting(IQueryable<Transaction> query, TransactionSortOrder sortBy)
    {
        return sortBy switch
        {
            TransactionSortOrder.Oldest => query.OrderBy(t => t.CreatedAt),
            TransactionSortOrder.AmountHighToLow => query.OrderByDescending(t => t.Amount),
            TransactionSortOrder.AmountLowToHigh => query.OrderBy(t => t.Amount),
            _ => query.OrderByDescending(t => t.CreatedAt)
        };
    }
}
