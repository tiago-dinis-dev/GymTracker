namespace Application.Dtos;

public record ExercisePerformedDto
{
    public Guid ExerciseId { get; init; }
    public List<SetRecordDto> Sets { get; init; } = new();
}
