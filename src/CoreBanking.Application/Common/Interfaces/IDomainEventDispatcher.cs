using CoreBanking.Domain.Common;

namespace CoreBanking.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
