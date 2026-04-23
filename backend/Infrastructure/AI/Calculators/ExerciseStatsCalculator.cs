using Common.AI.Models;
using Common.AI.Observations;

namespace Infrastructure.AI.Calculators;

public static class ExerciseStatsCalculator
{
    public static ExerciseStats Update(ExerciseStats? current, ExerciseAddedObservation observation)
    {
        current ??= new ExerciseStats
        {
            UserId = observation.UserId,
            ExerciseName = observation.Metadata?.Name ?? "Unknown Exercise",
            MuscleGroup = observation.MuscleGroup
        };

        var setsCount = observation.Metadata?.Sets?.Count ?? 0;
        var totalSets = current.TotalSets + setsCount;

        var repsSum = observation.Metadata?.Sets?.Sum(s => s.Reps) ?? 0;
        var totalReps = current.TotalReps + repsSum;

        var totalVolume = current.TotalVolume + observation.VolumeData;
        var averageWeight = totalReps > 0 ? totalVolume / totalReps : 0;

        return current with
        {
            TotalSets = totalSets,
            TotalReps = totalReps,
            TotalVolume = totalVolume,
            AverageWeight = averageWeight
        };
    }
}
