using CoreBanking.Domain.Entities;

namespace CoreBanking.Application.Common.Interfaces;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey?> GetByKeyAsync(string key);
    Task AddAsync(IdempotencyKey idempotencyKey);
}
