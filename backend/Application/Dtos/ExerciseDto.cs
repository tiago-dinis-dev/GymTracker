namespace Application.Dtos;

public record ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string MuscleGroup { get; init; } = string.Empty;
    public string? Difficulty { get; init; }
    public string? Description { get; init; }
}