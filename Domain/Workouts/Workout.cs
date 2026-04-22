using Domain.Common;
using Domain.Common.Events;

namespace Domain.Workouts;

public class Workout : AggregateRoot
{
    private readonly List<ExercisePerformed> _exercises = new();
    public Guid UserId { get; private set; }
    public DateTime Date { get; private set; }
    public WorkoutStatus Status { get; private set; }
    public IReadOnlyCollection<ExercisePerformed> Exercises => _exercises;

    private Workout() { }
    public Workout(Guid userId, DateTime date)
    {
        UserId = userId;
        Date =  date;
        Status = WorkoutStatus.InProgress;
    }

    public void AddExercise(Guid exerciseId, IEnumerable<SetRecord> sets)
    {
        EnsureWorkoutIsNotCompleted();

        if (_exercises.Any(e => e.ExerciseId == exerciseId))
        {
            throw new InvalidOperationException("Exercise already exists.");
        }

        var exercise = new ExercisePerformed(exerciseId);

        foreach(var set in sets)
        {
            exercise.AddSet(set.SetIndex, set.Reps, set.Weight, set.Estimated1Rm);
        }

        _exercises.Add(exercise);
        int orderInWorkout = _exercises.Count;

        RaiseEvent(new ExerciseAddedToWorkout(
            workoutId: Id,
            userId: UserId,
            exercise: exercise,
            orderInWorkout: orderInWorkout
        ));
    }

    public float CalculateTotalVolume()
    {
        return Exercises.Sum(e => e.CalculateVolume());
    }

    public void UpdateExerciseSet(Guid exerciseId, int index, int? reps = null, float? weight = null, decimal? estimated1Rm = null)
    {
        EnsureWorkoutIsNotCompleted();

        var exercise = _exercises.FirstOrDefault(e => e.ExerciseId == exerciseId)
            ?? throw new InvalidOperationException("Exercise not found in workout.");

        exercise.UpdateSet(index, reps, weight, estimated1Rm);
    }

    public void Complete()
    {
        if (Status == WorkoutStatus.Completed)
            return;

        if (_exercises.Count == 0)
            throw new InvalidOperationException("Cannot complete a workout with no exercises.");

        Status = WorkoutStatus.Completed;

        RaiseEvent(new WorkoutCompleted(Id, UserId, Date, _exercises));
    }

    private void EnsureWorkoutIsNotCompleted()
    {
        if (Status == WorkoutStatus.Completed)
            throw new InvalidOperationException("Workout is already completed.");
    }
}
