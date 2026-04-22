using Application.Workouts.Validators;
using Common.Workouts;

namespace Application.Tests.Workouts.Validators;

public class CreateWorkoutRequestValidatorTests
{
    private readonly CreateWorkoutRequestValidator _validator = new();

    [Fact]
    public void Validate_TodayDate_IsValid()
    {
        var request = new CreateWorkoutRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_FutureDate_IsInvalid()
    {
        var request = new CreateWorkoutRequest { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_DefaultDate_IsInvalid()
    {
        var request = new CreateWorkoutRequest { Date = default };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
