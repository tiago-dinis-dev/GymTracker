namespace Common.AI.Observations;

public record WorkoutCompletedObservation
{
    public Guid WorkoutId { get; init; }
    public Guid UserId { get; init; }
    public DateTime Timestamp { get; init; }
    public decimal TotalVolume { get; init; }
    public TimeSpan TotalDuration { get; init; }
    public int ExerciseCount { get; init; }
    public DayOfWeek WorkoutDay => Timestamp.DayOfWeek;
}
