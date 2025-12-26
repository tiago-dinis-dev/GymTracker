namespace Api.Workouts;

public record SetRecord
{
    public int Reps { get; init; }
    public float Weight { get; init; }
}

public record AddExerciseRequest
{
    public Guid ExerciseId { get; init; }
    public List<SetRecord> Sets { get; init; } = new();
}
