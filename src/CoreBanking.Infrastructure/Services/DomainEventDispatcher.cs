using MediatR;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Domain.Common;

namespace CoreBanking.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _mediator.Publish(domainEvent, cancellationToken);
    }
}
