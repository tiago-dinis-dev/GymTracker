using Domain.Common;
using Domain.Workouts;

namespace Domain.Tests.Workouts;

public class ExercisePerformedTests
{
    private static ExercisePerformed CreateExercise() => new(Guid.NewGuid());

    [Fact]
    public void AddSet_ValidSet_AddsSuccessfully()
    {
        var exercise = CreateExercise();

        exercise.AddSet(0, 10, 100f, 120m);

        Assert.Single(exercise.Sets);
        Assert.Equal(10, exercise.Sets.First().Reps);
    }

    [Fact]
    public void AddSet_DuplicateIndex_ThrowsDomainRuleViolation()
    {
        var exercise = CreateExercise();
        exercise.AddSet(0, 10, 100f, 120m);

        Assert.Throws<DomainRuleViolationException>(() => exercise.AddSet(0, 8, 90f, 110m));
    }

    [Theory]
    [InlineData(0, 100f, 120)]
    [InlineData(10, 0f, 120)]
    [InlineData(10, 100f, 0)]
    public void AddSet_InvalidParameters_ThrowsDomainRuleViolation(int reps, float weight, int estimated1Rm)
    {
        var exercise = CreateExercise();

        Assert.Throws<DomainRuleViolationException>(() => exercise.AddSet(0, reps, weight, estimated1Rm));
    }

    [Fact]
    public void UpdateSet_ValidUpdate_UpdatesValues()
    {
        var exercise = CreateExercise();
        exercise.AddSet(0, 10, 100f, 120m);

        exercise.UpdateSet(0, reps: 12, weight: 110f, estimated1Rm: 130m);

        var set = exercise.Sets.First();
        Assert.Equal(12, set.Reps);
        Assert.Equal(110f, set.Weight);
        Assert.Equal(130m, set.Estimated1Rm);
    }

    [Fact]
    public void UpdateSet_PartialUpdate_KeepsExistingValues()
    {
        var exercise = CreateExercise();
        exercise.AddSet(0, 10, 100f, 120m);

        exercise.UpdateSet(0, reps: 12);

        var set = exercise.Sets.First();
        Assert.Equal(12, set.Reps);
        Assert.Equal(100f, set.Weight);
        Assert.Equal(120m, set.Estimated1Rm);
    }

    [Fact]
    public void UpdateSet_NonExistentIndex_ThrowsInvalidOperation()
    {
        var exercise = CreateExercise();
        exercise.AddSet(0, 10, 100f, 120m);

        Assert.Throws<InvalidOperationException>(() => exercise.UpdateSet(5, reps: 12));
    }

    [Fact]
    public void CalculateVolume_ReturnsSumOfSetVolumes()
    {
        var exercise = CreateExercise();
        exercise.AddSet(0, 10, 100f, 120m);
        exercise.AddSet(1, 8, 90f, 110m);

        var volume = exercise.CalculateVolume();

        Assert.Equal(10 * 100f + 8 * 90f, volume);
    }

    [Fact]
    public void IntensityPercent1Rm_ReturnsAverageAcrossSets()
    {
        var exercise = CreateExercise();
        exercise.AddSet(0, 10, 80f, 100m);
        exercise.AddSet(1, 8, 70f, 100m);

        var expected = (80m / 100m * 100 + 70m / 100m * 100) / 2;
        Assert.Equal(expected, exercise.IntensityPercent1Rm);
    }
}
