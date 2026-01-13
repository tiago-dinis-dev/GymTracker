using Application.Common.Interfaces;
using Domain.Common.Events;

namespace Infrastructure.BackgroundJobs;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));

            if (_serviceProvider.GetService(handlerType) is System.Collections.IEnumerable handlers)
            {
                foreach (var handler in handlers)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)domainEvent, ct);
                }
            }
        }
    }
}
