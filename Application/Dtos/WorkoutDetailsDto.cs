using Domain.Workouts;

namespace Application.Dtos;

public record WorkoutDetailsDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public WorkoutStatus Status { get; init; }
    public List<ExercisePerformedDto> Exercises { get; init; } = new();
}
