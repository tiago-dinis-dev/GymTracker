using Domain.Exercises;
using Domain.Workouts;

namespace Common.AI.Observations;

public record ExerciseAddedObservation
{
    public Guid WorkoutId { get; init; }
    public Guid UserId { get; init; }
    public ExerciseDetails? Metadata { get; init; }
    public decimal VolumeData { get; init; }
    public int OrderInWorkout { get; init; }
    public DateTime Timestamp { get; init; }

    public MuscleGroup MuscleGroup => Metadata!.MuscleGroup;
}

public record ExerciseDetails
{
    public string? Name { get; init; }
    public IReadOnlyCollection<SetRecord>? Sets { get; init; }
    public MuscleGroup MuscleGroup { get; init; }
}