using Common.AI.Models;
using Common.AI.Observations;

namespace Infrastructure.AI.Calculators;

public static class WorkoutStatsCalculator
{
    public static UserWorkoutStats Update(UserWorkoutStats current, WorkoutCompletedObservation observation)
    {
        var newStats = new UserWorkoutStats(
            current.UserId,
            current.TotalWorkouts + 1,
            current.TotalExercises + observation.ExerciseCount,
            current.TotalVolume + observation.TotalVolume,
            (current.TotalVolume + observation.TotalVolume) / (current.TotalWorkouts + 1),
            TimeSpan.FromTicks(
                ((current.AverageWorkoutDuration.Ticks * current.TotalWorkouts) + observation.TotalDuration.Ticks)
                / (current.TotalWorkouts + 1)
            ),
            observation.Timestamp > current.LastWorkoutAt ? observation.Timestamp : current.LastWorkoutAt
        );

        return newStats;
    }
}
