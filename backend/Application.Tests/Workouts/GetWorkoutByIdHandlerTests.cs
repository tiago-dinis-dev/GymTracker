using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Application.Workouts;
using Domain.Workouts;
using Moq;

namespace Application.Tests.Workouts;

public class GetWorkoutByIdHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly GetWorkoutByIdHandler _handler;

    public GetWorkoutByIdHandlerTests()
    {
        _handler = new GetWorkoutByIdHandler(_workoutRepo.Object);
    }

    [Fact]
    public async Task HandleAsync_WorkoutExists_ReturnsDto()
    {
        var workout = new Workout(Guid.NewGuid(), DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 120m)]);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);

        var result = await _handler.HandleAsync(new GetWorkoutByIdQuery(workout.Id));

        Assert.Equal(workout.Id, result.Id);
        Assert.Single(result.Exercises);
        Assert.Equal("InProgress", result.Status);
    }

    [Fact]
    public async Task HandleAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Workout?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new GetWorkoutByIdQuery(Guid.NewGuid())));
    }
}
