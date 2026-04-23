using Domain.Common;
using Common.Exercises;

namespace Domain.Exercises;

public class Exercise : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public MuscleGroup MuscleGroup { get; private set; }
    public string? Difficulty { get; private set; }
    public string? Description { get; private set; }

    private Exercise() { }

    public Exercise(string name, MuscleGroup muscleGroup, string? difficulty = null, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        MuscleGroup = muscleGroup;
        Difficulty = difficulty;
        Description = description;
    }

    public void UpdateDetails(string? difficulty = null, string? description = null)
    {
        if (difficulty != null) Difficulty = difficulty;
        if (description != null) Description = description;
    }
}
