using Domain.Common;

namespace Domain.Workouts;

public class ExercisePerformed
{
    private readonly List<SetRecord> _sets = new();
    public Guid ExerciseId { get; private set; }
    public decimal IntensityPercent1Rm => Sets.Average(s => s.IntensityPercent1Rm);
    public IReadOnlyCollection<SetRecord> Sets => _sets;

    private ExercisePerformed() { }
    public ExercisePerformed(Guid exerciseId)
    {
        ExerciseId = exerciseId; 
    }

    public void AddSet(int index, int reps, float weight, decimal estimated1Rm)
    {
        ValidateSet(reps, weight, estimated1Rm);

        if (_sets.Any(s => s.SetIndex == index))
        {
            throw new DomainRuleViolationException($"Set with index {index} already exists in this exercise.");
        }

        _sets.Add(new SetRecord(index, reps, weight, estimated1Rm));
    }

    public void UpdateSet(int index, int? reps = null, float? weight = null, decimal? estimated1Rm = null)
    {
        var currentSetIndex = _sets.FindIndex(s => s.SetIndex == index);
        if (currentSetIndex == -1)
        {
            throw new InvalidOperationException("Set not found.");
        }

        var currentSet = _sets[currentSetIndex];

        int newReps = reps ?? currentSet.Reps;
        float newWeight = weight ?? currentSet.Weight;
        decimal newEstimated1Rm = estimated1Rm ?? currentSet.Estimated1Rm;

        ValidateSet(newReps, newWeight, newEstimated1Rm);

        _sets[currentSetIndex] = new SetRecord(index, newReps, newWeight, newEstimated1Rm);
    }

    public float CalculateVolume()
    {
        return _sets.Sum(s => s.CalculateVolume());
    }

    private static void ValidateSet(int reps, float weight, decimal estimated1Rm)
    {
        if (reps <= 0 || weight <= 0 || estimated1Rm <= 0)
        {
            throw new DomainRuleViolationException("Invalid set parameters.");
        }
    }
}
