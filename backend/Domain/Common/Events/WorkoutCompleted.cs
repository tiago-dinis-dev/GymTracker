using Domain.Workouts;

namespace Domain.Common.Events;

public record WorkoutCompleted(Guid WorkoutId, Guid UserId, DateTime StartedAt,
    DateTime OccurredAt,
    List<ExercisePerformed> Exercises) : IDomainEvent
{
    public WorkoutCompleted(Guid workoutId, Guid userId, DateTime startedAt,
        List<ExercisePerformed> exercises) : this(workoutId, userId, startedAt,
            DateTime.UtcNow, exercises)
    {
    }
}
