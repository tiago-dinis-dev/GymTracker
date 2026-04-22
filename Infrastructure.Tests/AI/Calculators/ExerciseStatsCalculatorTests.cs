using Common.AI.Observations;
using Common.Exercises;
using Common.Workouts;
using Infrastructure.AI.Calculators;

namespace Infrastructure.Tests.AI.Calculators;

public class ExerciseStatsCalculatorTests
{
    private static ExerciseAddedObservation CreateObservation(int sets = 1, int reps = 10, float weight = 100f) =>
        new()
        {
            WorkoutId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            VolumeData = sets * reps * (decimal)weight,
            Metadata = new ExerciseDetails
            {
                Name = "Bench Press",
                MuscleGroup = MuscleGroup.Chest,
                IntensityPercent1Rm = 80m,
                Sets = Enumerable.Range(0, sets).Select(i => new SetInfo { Index = i, Reps = reps, Weight = weight, Estimated1Rm = 120m }).ToList()
            }
        };

    [Fact]
    public void Update_NullCurrent_CreatesNewStats()
    {
        var obs = CreateObservation();

        var result = ExerciseStatsCalculator.Update(null, obs);

        Assert.Equal("Bench Press", result.ExerciseName);
        Assert.Equal(MuscleGroup.Chest, result.MuscleGroup);
        Assert.Equal(1, result.TotalSets);
        Assert.Equal(10, result.TotalReps);
    }

    [Fact]
    public void Update_ExistingStats_AccumulatesValues()
    {
        var obs = CreateObservation();
        var existing = ExerciseStatsCalculator.Update(null, obs);

        var result = ExerciseStatsCalculator.Update(existing, CreateObservation(sets: 2, reps: 8, weight: 90f));

        Assert.Equal(3, result.TotalSets);
        Assert.Equal(10 + 16, result.TotalReps);
    }

    [Fact]
    public void Update_CalculatesAverageWeight()
    {
        var obs = CreateObservation(sets: 1, reps: 10, weight: 100f);

        var result = ExerciseStatsCalculator.Update(null, obs);

        Assert.Equal(1000m / 10m, result.AverageWeight);
    }
}
