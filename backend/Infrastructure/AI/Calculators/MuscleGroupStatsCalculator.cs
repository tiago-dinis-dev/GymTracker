using Common.AI.Models;
using Common.AI.Observations;

namespace Infrastructure.AI.Calculators;

public static class MuscleGroupStatsCalculator
{
    public static MuscleGroupStats Update(MuscleGroupStats? current, ExerciseAddedObservation observation)
    {
        current ??= new MuscleGroupStats
        {
            UserId = observation.UserId,
            MuscleGroup = observation.MuscleGroup
        };

        int sets = observation.Metadata?.Sets?.Count ?? 0;
        int reps = observation.Metadata?.Sets?.Sum(s => s.Reps) ?? 0;
        decimal volume = observation.VolumeData;
        decimal intensity = observation.Metadata?.IntensityPercent1Rm ?? 0;

        return current with 
        { 
            TotalExercises = current.TotalExercises + 1,
            TotalSets = current.TotalSets + sets,
            TotalReps = current.TotalReps + reps,
            TotalVolume = current.TotalVolume + volume,
            AverageIntensity = (current.AverageIntensity * current.TotalExercises + intensity) / (current.TotalExercises + 1)
        };
    }
}
