namespace Application.Dtos;

public record SetRecordDto
{
    public int Reps { get; init; }
    public float Weight { get; init; }
    public decimal IntensityPercent1Rm { get; init; }
}
