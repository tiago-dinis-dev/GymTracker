using Common.Exercises;

namespace Common.AI.Models;

public record ExerciseStats
{
    public Guid UserId { get; init; }
    public string ExerciseName { get; init; } = null!;
    public MuscleGroup MuscleGroup { get; init; }
    public int TotalSets { get; init; }
    public int TotalReps { get; init; }
    public decimal TotalVolume { get; init; }
    public decimal AverageWeight { get; init; }
}
