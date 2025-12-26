using Domain.Common;

namespace Domain.Workouts;

public class Workout : AggregateRoot
{
    public readonly List<ExercisePerformed> _exercises = new();
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

    public void AddExercise(ExercisePerformed exercise)
    {
        if (Status == WorkoutStatus.Completed)
            throw new InvalidOperationException("Cannot add exercises to a workout that is completed.");

        _exercises.Add(new ExercisePerformed(exercise.ExerciseId));
    }

    public float CalculateTotalVolume()
    {
        return Exercises.Sum(e => e.CalculateVolume());
    }

    public void Complete()
    {
        if (_exercises.Count == 0)
            throw new InvalidOperationException("Cannot complete a workout with no exercises.");

        Status = WorkoutStatus.Completed;
    }
}
