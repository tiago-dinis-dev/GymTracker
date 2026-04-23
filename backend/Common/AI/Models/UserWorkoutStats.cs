namespace Common.AI.Models;

public record UserWorkoutStats(
    Guid UserId,
    int TotalWorkouts,
    int TotalExercises,
    decimal TotalVolume,
    decimal AverageVolumePerWorkout,
    TimeSpan AverageWorkoutDuration,
    DateTime LastWorkoutAt
);
