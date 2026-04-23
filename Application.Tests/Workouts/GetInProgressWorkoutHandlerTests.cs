using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Workouts;
using Domain.Workouts;
using Moq;

namespace Application.Tests.Workouts;

public class GetInProgressWorkoutHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly GetInProgressWorkoutHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public GetInProgressWorkoutHandlerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        _handler = new GetInProgressWorkoutHandler(_workoutRepo.Object, _userContext.Object);
    }

    [Fact]
    public async Task HandleAsync_InProgressWorkoutExists_ReturnsDto()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 8, 80f, 100m)]);
        _workoutRepo.Setup(x => x.GetInProgressWorkoutAsync(_userId)).ReturnsAsync(workout);

        var result = await _handler.HandleAsync();

        Assert.NotNull(result);
        Assert.Equal(workout.Id, result.Id);
        Assert.Equal("InProgress", result.Status);
        Assert.Single(result.Exercises);
    }

    [Fact]
    public async Task HandleAsync_NoInProgressWorkout_ReturnsNull()
    {
        _workoutRepo.Setup(x => x.GetInProgressWorkoutAsync(_userId)).ReturnsAsync((Workout?)null);

        var result = await _handler.HandleAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_UsesCurrentUserId()
    {
        _workoutRepo.Setup(x => x.GetInProgressWorkoutAsync(_userId)).ReturnsAsync((Workout?)null);

        await _handler.HandleAsync();

        _workoutRepo.Verify(x => x.GetInProgressWorkoutAsync(_userId), Times.Once);
    }
}
