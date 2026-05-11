namespace Common.AI.Models;

public record WorkoutHistorySummary(
    Guid WorkoutId,
    DateTime Date,
    int ExerciseCount,
    decimal TotalVolume,
    List<ExerciseHistorySummary> Exercises
);

public record ExerciseHistorySummary(
    string ExerciseName,
    string MuscleGroup,
    int SetCount,
    int TotalReps,
    decimal TotalVolume,
    decimal AverageWeight
);
