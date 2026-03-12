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

    public void AddSet(int reps, float weight, decimal estimated1Rm)
    {
        _sets.Add(new SetRecord(reps, weight, estimated1Rm));
    }

    public float CalculateVolume()
    {
        return _sets.Sum(s => s.CalculateVolume());
    }
}
