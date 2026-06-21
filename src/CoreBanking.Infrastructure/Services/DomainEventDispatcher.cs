using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Common;

namespace CoreBanking.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Domain events will be dispatched via MediatR when INotification handlers are added.
        // For now, this is a no-op since there are no event handlers.
        return Task.CompletedTask;
    }
}
