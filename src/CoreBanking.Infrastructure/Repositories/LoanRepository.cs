using Microsoft.EntityFrameworkCore;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;
using CoreBanking.Application.Loans.Queries;
using CoreBanking.Domain.Entities;
using CoreBanking.Infrastructure.Persistence;

namespace CoreBanking.Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly BankingDbContext _context;

    public LoanRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(Guid id)
    {
        return await _context.Loans.FindAsync(id);
    }

    public async Task<Loan?> GetByLoanNumberAsync(string loanNumber)
    {
        return await _context.Loans
            .FirstOrDefaultAsync(l => l.LoanNumber.Value == loanNumber);
    }

    public async Task<PaginatedResult<Loan>> GetByAccountIdAsync(Guid accountId, LoanSortOrder sortBy, int page, int pageSize)
    {
        var query = _context.Loans
            .Where(l => l.AccountId == accountId);
        query = ApplySorting(query, sortBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Loan>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PaginatedResult<Loan>> GetAllAsync(LoanSortOrder sortBy, int page, int pageSize)
    {
        var query = _context.Loans.AsQueryable();
        query = ApplySorting(query, sortBy);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Loan>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task AddAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        await _context.Loans.AddAsync(loan, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Loan> ApplySorting(IQueryable<Loan> query, LoanSortOrder sortBy)
    {
        return sortBy switch
        {
            LoanSortOrder.Oldest => query.OrderBy(l => l.CreatedAt),
            LoanSortOrder.AmountHighToLow => query.OrderByDescending(l => l.PrincipalAmount.Amount),
            LoanSortOrder.AmountLowToHigh => query.OrderBy(l => l.PrincipalAmount.Amount),
            LoanSortOrder.Status => query.OrderBy(l => l.Status),
            _ => query.OrderByDescending(l => l.CreatedAt)
        };
    }
}
