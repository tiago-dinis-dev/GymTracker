using Domain.Common.Events;
using Domain.Workouts;

namespace Domain.Tests.Workouts;

public class WorkoutTests
{
    private static Workout CreateWorkout() => new(Guid.NewGuid(), DateTime.UtcNow);
    private static List<SetRecord> CreateSets() => [new SetRecord(0, 10, 100f, 120m)];

    [Fact]
    public void Constructor_SetsStatusToInProgress()
    {
        var workout = CreateWorkout();

        Assert.Equal(WorkoutStatus.InProgress, workout.Status);
    }

    [Fact]
    public void AddExercise_ValidExercise_AddsSuccessfully()
    {
        var workout = CreateWorkout();
        var exerciseId = Guid.NewGuid();

        workout.AddExercise(exerciseId, CreateSets());

        Assert.Single(workout.Exercises);
        Assert.Equal(exerciseId, workout.Exercises.First().ExerciseId);
    }

    [Fact]
    public void AddExercise_DuplicateExercise_ThrowsInvalidOperation()
    {
        var workout = CreateWorkout();
        var exerciseId = Guid.NewGuid();
        workout.AddExercise(exerciseId, CreateSets());

        Assert.Throws<InvalidOperationException>(() => workout.AddExercise(exerciseId, CreateSets()));
    }

    [Fact]
    public void AddExercise_CompletedWorkout_ThrowsInvalidOperation()
    {
        var workout = CreateWorkout();
        workout.AddExercise(Guid.NewGuid(), CreateSets());
        workout.Complete();

        Assert.Throws<InvalidOperationException>(() => workout.AddExercise(Guid.NewGuid(), CreateSets()));
    }

    [Fact]
    public void AddExercise_RaisesExerciseAddedToWorkoutEvent()
    {
        var workout = CreateWorkout();

        workout.AddExercise(Guid.NewGuid(), CreateSets());

        Assert.Single(workout.DomainEvents);
        Assert.IsType<ExerciseAddedToWorkout>(workout.DomainEvents.First());
    }

    [Fact]
    public void UpdateExerciseSet_ValidUpdate_Succeeds()
    {
        var workout = CreateWorkout();
        var exerciseId = Guid.NewGuid();
        workout.AddExercise(exerciseId, CreateSets());

        workout.UpdateExerciseSet(exerciseId, 0, reps: 12);

        var set = workout.Exercises.First().Sets.First();
        Assert.Equal(12, set.Reps);
    }

    [Fact]
    public void UpdateExerciseSet_NonExistentExercise_ThrowsInvalidOperation()
    {
        var workout = CreateWorkout();

        Assert.Throws<InvalidOperationException>(() => workout.UpdateExerciseSet(Guid.NewGuid(), 0, reps: 12));
    }

    [Fact]
    public void UpdateExerciseSet_CompletedWorkout_ThrowsInvalidOperation()
    {
        var workout = CreateWorkout();
        var exerciseId = Guid.NewGuid();
        workout.AddExercise(exerciseId, CreateSets());
        workout.Complete();

        Assert.Throws<InvalidOperationException>(() => workout.UpdateExerciseSet(exerciseId, 0, reps: 12));
    }

    [Fact]
    public void Complete_WithExercises_SetsStatusToCompleted()
    {
        var workout = CreateWorkout();
        workout.AddExercise(Guid.NewGuid(), CreateSets());

        workout.Complete();

        Assert.Equal(WorkoutStatus.Completed, workout.Status);
    }

    [Fact]
    public void Complete_WithExercises_RaisesWorkoutCompletedEvent()
    {
        var workout = CreateWorkout();
        workout.AddExercise(Guid.NewGuid(), CreateSets());

        workout.Complete();

        Assert.Contains(workout.DomainEvents, e => e is WorkoutCompleted);
    }

    [Fact]
    public void Complete_NoExercises_ThrowsInvalidOperation()
    {
        var workout = CreateWorkout();

        Assert.Throws<InvalidOperationException>(() => workout.Complete());
    }

    [Fact]
    public void Complete_AlreadyCompleted_DoesNotThrow()
    {
        var workout = CreateWorkout();
        workout.AddExercise(Guid.NewGuid(), CreateSets());
        workout.Complete();

        workout.Complete(); // should be idempotent

        Assert.Equal(WorkoutStatus.Completed, workout.Status);
    }

    [Fact]
    public void CalculateTotalVolume_ReturnsSumAcrossExercises()
    {
        var workout = CreateWorkout();
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 120m)]);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 8, 80f, 100m)]);

        var volume = workout.CalculateTotalVolume();

        Assert.Equal(10 * 100f + 8 * 80f, volume);
    }

    [Fact]
    public void ClearDomainEvents_ClearsAllEvents()
    {
        var workout = CreateWorkout();
        workout.AddExercise(Guid.NewGuid(), CreateSets());
        Assert.NotEmpty(workout.DomainEvents);

        workout.ClearDomainEvents();

        Assert.Empty(workout.DomainEvents);
    }
}
