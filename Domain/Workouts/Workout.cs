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
        Status = WorkoutStatus.Planned;
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
            exercise.AddSet(set.Reps, set.Weight);
        }

        _exercises.Add(exercise);
    }

    public void AddSet(Guid exerciseId, int reps, float weight)
    {
        EnsureWorkoutIsNotCompleted();

        var exercise = _exercises.FirstOrDefault(x => x.ExerciseId == exerciseId)
            ?? throw new InvalidOperationException("Exercise not found in workout");

        exercise.AddSet(reps, weight);
    }

    public float CalculateTotalVolume()
    {
        return Exercises.Sum(e => e.CalculateVolume());
    }

    public void Complete()
    {
        if (Status == WorkoutStatus.Completed)
            return;

        if (_exercises.Count == 0)
            throw new InvalidOperationException("Cannot complete a workout with no exercises.");

        Status = WorkoutStatus.Completed;

        RaiseEvent(new WorkoutCompleted(Id, UserId));
    }

    private void EnsureWorkoutIsNotCompleted()
    {
        if (Status == WorkoutStatus.Completed)
            throw new InvalidOperationException("Workout is already completed.");
    }
}
