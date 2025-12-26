namespace Api.Workouts;

public record CreateWorkoutRequest
{
    public DateOnly Date { get; init; }
}
