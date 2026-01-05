using Domain.Workouts;

namespace Application.Dtos;

public record WorkoutHistoryDto
{
    public DateTime Date { get; set; }
    public required string Status { get; set; }
    public float TotalVolume { get; set; }
}
