namespace Application.Common.Caching;

public static class CacheKeys
{
    public static string WorkoutHistory(Guid userId) => $"workouts:history:{userId}";

    public static string WorkoutDetails(Guid workoutId) => $"workouts:details:{workoutId}";
}
