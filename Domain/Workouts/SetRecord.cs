namespace Domain.Workouts;

public class SetRecord
{
    public int Reps { get; private set; }
    public float Weight { get; private set; }
    public decimal Estimated1Rm { get; private set; }

    public decimal IntensityPercent1Rm => (decimal)Weight / Estimated1Rm * 100;

    private SetRecord() { }
    public SetRecord(int reps, float weight, decimal estimated1Rm)
    {
        Reps = reps;
        Weight = weight;
        Estimated1Rm = estimated1Rm;
    }

    public float CalculateVolume()
    {
        return Reps * Weight;
    }
}
