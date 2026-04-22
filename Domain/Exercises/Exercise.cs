using Domain.Common;
using Common.Exercises;

namespace Domain.Exercises;

public class Exercise : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public MuscleGroup MuscleGroup { get; private set; }

    private Exercise() { }

    public Exercise(string name, MuscleGroup muscleGroup)
    {
        Id = Guid.NewGuid();
        Name = name;
        MuscleGroup = muscleGroup;
    }
}
