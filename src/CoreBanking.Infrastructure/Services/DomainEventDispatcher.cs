using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Common;

namespace CoreBanking.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _publisher.Publish(domainEvent, cancellationToken);
    }
}
