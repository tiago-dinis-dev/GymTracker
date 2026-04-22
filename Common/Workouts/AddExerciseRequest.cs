namespace Common.Workouts;

public record AddExerciseRequest
{
    public required Guid ExerciseId { get; init; }
    public required List<SetInfo> Sets { get; init; } = new();
}
