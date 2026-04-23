namespace Common.Workouts;

public record CreateWorkoutRequest
{
    public DateOnly Date { get; init; }
}
