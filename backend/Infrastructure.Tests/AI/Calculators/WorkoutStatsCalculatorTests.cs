using Common.AI.Models;
using Common.AI.Observations;
using Infrastructure.AI.Calculators;

namespace Infrastructure.Tests.AI.Calculators;

public class WorkoutStatsCalculatorTests
{
    private static UserWorkoutStats EmptyStats(Guid userId) =>
        new(userId, 0, 0, 0m, 0m, TimeSpan.Zero, DateTime.MinValue);

    [Fact]
    public void Update_FirstWorkout_SetsAllValues()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var obs = new WorkoutCompletedObservation
        {
            WorkoutId = Guid.NewGuid(),
            UserId = userId,
            Timestamp = now,
            TotalVolume = 5000m,
            TotalDuration = TimeSpan.FromMinutes(60),
            ExerciseCount = 5
        };

        var result = WorkoutStatsCalculator.Update(EmptyStats(userId), obs);

        Assert.Equal(1, result.TotalWorkouts);
        Assert.Equal(5, result.TotalExercises);
        Assert.Equal(5000m, result.TotalVolume);
        Assert.Equal(5000m, result.AverageVolumePerWorkout);
        Assert.Equal(TimeSpan.FromMinutes(60), result.AverageWorkoutDuration);
        Assert.Equal(now, result.LastWorkoutAt);
    }

    [Fact]
    public void Update_SecondWorkout_AveragesCorrectly()
    {
        var userId = Guid.NewGuid();
        var first = WorkoutStatsCalculator.Update(EmptyStats(userId), new WorkoutCompletedObservation
        {
            WorkoutId = Guid.NewGuid(),
            UserId = userId,
            Timestamp = DateTime.UtcNow.AddHours(-2),
            TotalVolume = 4000m,
            TotalDuration = TimeSpan.FromMinutes(40),
            ExerciseCount = 4
        });

        var result = WorkoutStatsCalculator.Update(first, new WorkoutCompletedObservation
        {
            WorkoutId = Guid.NewGuid(),
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            TotalVolume = 6000m,
            TotalDuration = TimeSpan.FromMinutes(60),
            ExerciseCount = 6
        });

        Assert.Equal(2, result.TotalWorkouts);
        Assert.Equal(10, result.TotalExercises);
        Assert.Equal(10000m, result.TotalVolume);
        Assert.Equal(5000m, result.AverageVolumePerWorkout);
        Assert.Equal(TimeSpan.FromMinutes(50), result.AverageWorkoutDuration);
    }
}
