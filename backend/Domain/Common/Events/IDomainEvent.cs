namespace Domain.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
