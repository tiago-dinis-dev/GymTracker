namespace Common.Workouts;

public record SetInfo
{
    public required int Index { get; set; }
    public int Reps { get; init; }
    public float Weight { get; init; }
    public decimal Estimated1Rm { get; init; }
}
