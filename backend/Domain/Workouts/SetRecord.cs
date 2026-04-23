namespace Domain.Workouts;

public class SetRecord
{
    public int SetIndex { get; private set; }
    public int Reps { get; private set; }
    public float Weight { get; private set; }
    public decimal Estimated1Rm { get; private set; }

    public decimal IntensityPercent1Rm => Math.Round((decimal)Weight / Estimated1Rm * 100, 2);

    private SetRecord() { }
    public SetRecord(int index, int reps, float weight, decimal estimated1Rm)
    {
        SetIndex = index;
        Reps = reps;
        Weight = weight;
        Estimated1Rm = estimated1Rm;
    }

    public float CalculateVolume()
    {
        return Reps * Weight;
    }
}
