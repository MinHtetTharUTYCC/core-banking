using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
