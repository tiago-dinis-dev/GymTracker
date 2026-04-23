using Application.Workouts.Validators;
using Common.Workouts;

namespace Application.Tests.Workouts.Validators;

public class AddExerciseRequestValidatorTests
{
    private readonly AddExerciseRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var request = new AddExerciseRequest
        {
            ExerciseId = Guid.NewGuid(),
            Sets = [new SetInfo { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }]
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyExerciseId_IsInvalid()
    {
        var request = new AddExerciseRequest
        {
            ExerciseId = Guid.Empty,
            Sets = [new SetInfo { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }]
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_EmptySets_IsInvalid()
    {
        var request = new AddExerciseRequest
        {
            ExerciseId = Guid.NewGuid(),
            Sets = []
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ZeroReps_IsInvalid()
    {
        var request = new AddExerciseRequest
        {
            ExerciseId = Guid.NewGuid(),
            Sets = [new SetInfo { Index = 0, Reps = 0, Weight = 100f, Estimated1Rm = 120m }]
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_NegativeWeight_IsInvalid()
    {
        var request = new AddExerciseRequest
        {
            ExerciseId = Guid.NewGuid(),
            Sets = [new SetInfo { Index = 0, Reps = 10, Weight = -1f, Estimated1Rm = 120m }]
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
