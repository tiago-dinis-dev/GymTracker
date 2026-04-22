using Common.AI.Observations;
using Common.Exercises;
using Common.Workouts;
using Infrastructure.AI.Calculators;

namespace Infrastructure.Tests.AI.Calculators;

public class MuscleGroupStatsCalculatorTests
{
    private static ExerciseAddedObservation CreateObservation(int sets = 1, int reps = 10, float weight = 100f, decimal intensity = 80m) =>
        new()
        {
            WorkoutId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            VolumeData = sets * reps * (decimal)weight,
            Metadata = new ExerciseDetails
            {
                Name = "Bench Press",
                MuscleGroup = MuscleGroup.Chest,
                IntensityPercent1Rm = intensity,
                Sets = Enumerable.Range(0, sets).Select(i => new SetInfo { Index = i, Reps = reps, Weight = weight, Estimated1Rm = 120m }).ToList()
            }
        };

    [Fact]
    public void Update_NullCurrent_CreatesNewStats()
    {
        var obs = CreateObservation();

        var result = MuscleGroupStatsCalculator.Update(null, obs);

        Assert.Equal(MuscleGroup.Chest, result.MuscleGroup);
        Assert.Equal(1, result.TotalExercises);
        Assert.Equal(1, result.TotalSets);
        Assert.Equal(10, result.TotalReps);
    }

    [Fact]
    public void Update_ExistingStats_AccumulatesValues()
    {
        var obs1 = CreateObservation();
        var stats = MuscleGroupStatsCalculator.Update(null, obs1);

        var result = MuscleGroupStatsCalculator.Update(stats, CreateObservation(sets: 2, reps: 8, weight: 90f));

        Assert.Equal(2, result.TotalExercises);
        Assert.Equal(3, result.TotalSets);
        Assert.Equal(10 + 16, result.TotalReps);
    }

    [Fact]
    public void Update_CalculatesRunningAverageIntensity()
    {
        var stats = MuscleGroupStatsCalculator.Update(null, CreateObservation(intensity: 80m));

        var result = MuscleGroupStatsCalculator.Update(stats, CreateObservation(intensity: 60m));

        Assert.Equal(70m, result.AverageIntensity);
    }
}
