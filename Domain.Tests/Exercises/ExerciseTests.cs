using Common.Exercises;
using Domain.Exercises;

namespace Domain.Tests.Exercises;

public class ExerciseTests
{
    [Fact]
    public void Constructor_SetsNameAndMuscleGroup()
    {
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);

        Assert.Equal("Bench Press", exercise.Name);
        Assert.Equal(MuscleGroup.Chest, exercise.MuscleGroup);
        Assert.NotEqual(Guid.Empty, exercise.Id);
    }
}
