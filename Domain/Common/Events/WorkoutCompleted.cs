namespace Domain.Common.Events;

public record WorkoutCompleted(Guid WorkoutId, Guid UserId, DateTime OccurredAt) : IDomainEvent
{
    public WorkoutCompleted(Guid workoutId, Guid userId) : this(workoutId, userId, DateTime.UtcNow)
    {
    }
}
