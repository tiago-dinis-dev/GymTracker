using Domain.Workouts;

namespace Domain.Common.Events;

public record ExerciseAddedToWorkout(Guid WorkoutId, Guid userId, ExercisePerformed exercise, int orderInWorkout, DateTime OccurredAt) : IDomainEvent
{
    public ExerciseAddedToWorkout(Guid workoutId, Guid userId, ExercisePerformed exercise, int orderInWorkout) : this(workoutId, userId, exercise, orderInWorkout, DateTime.UtcNow)
    {
    }
}
