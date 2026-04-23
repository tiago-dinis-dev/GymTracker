using Application.Common.Interfaces;
using Domain.Common.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var @event in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                if (method == null) continue;

                var task = (Task?)method.Invoke(handler, [@event, ct]);
                if (task != null)
                    await task.ConfigureAwait(false);
            }
        }
    }
}
