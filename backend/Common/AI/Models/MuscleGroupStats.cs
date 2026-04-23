using Common.Exercises;

namespace Common.AI.Models;

public record MuscleGroupStats
{
    public Guid UserId { get; init; }
    public MuscleGroup MuscleGroup { get; init; }
    public int TotalExercises { get; init; }
    public decimal TotalVolume { get; init; }
    public int TotalSets { get; init; }
    public int TotalReps { get; init; }
    public decimal AverageIntensity { get; init; }
}
