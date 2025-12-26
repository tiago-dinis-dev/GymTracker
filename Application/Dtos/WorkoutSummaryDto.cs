using Domain.Workouts;

namespace Application.Dtos;

public record WorkoutSummaryDto
{
    public Guid WorkoutId { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public WorkoutStatus Status { get; init; }
    public int ExerciseCount { get; init; }
    public float TotalVolume { get; init; }
}
