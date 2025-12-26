namespace Api.Workouts;

public record CreateWorkoutResponse
{
    public Guid WorkoutId { get; init; }
    public CreateWorkoutResponse(Guid workoutId)
    {
        WorkoutId = workoutId;
    }
}
