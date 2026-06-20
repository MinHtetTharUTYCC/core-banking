using CoreBanking.Application.Common.Models;
using CoreBanking.Application.Loans.Queries;
using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Common.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid id);
    Task<Loan?> GetByLoanNumberAsync(string loanNumber);
    Task<PaginatedResult<Loan>> GetByAccountIdAsync(Guid accountId, LoanSortOrder sortBy, int page, int pageSize);
    Task<PaginatedResult<Loan>> GetAllAsync(LoanSortOrder sortBy, int page, int pageSize);
    Task AddAsync(Loan loan, CancellationToken cancellationToken = default);
    Task UpdateAsync(Loan loan, CancellationToken cancellationToken = default);
}
