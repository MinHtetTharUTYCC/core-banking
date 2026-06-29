using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Common;

namespace CoreBanking.Infrastructure.Services;

public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await mediator.Publish(domainEvent, cancellationToken);
    }
}
