using Domain.Common.Events;

namespace Application.Common.Interfaces;

public interface IDomainEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent, CancellationToken cancellationToken);
}
