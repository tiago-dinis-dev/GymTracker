using Api.Controllers;
using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Workouts;
using Domain.Workouts;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests.Controllers;

public class GetInProgressWorkoutControllerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly WorkoutsController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public GetInProgressWorkoutControllerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        var createHandler = new CreateWorkoutHandler(_workoutRepo.Object, _cache.Object, _userContext.Object);
        var completeHandler = new CompleteWorkoutHandler(_workoutRepo.Object, _userContext.Object);
        var getByIdHandler = new GetWorkoutByIdHandler(_workoutRepo.Object);
        var getInProgressHandler = new GetInProgressWorkoutHandler(_workoutRepo.Object, _userContext.Object);
        _controller = new WorkoutsController(createHandler, completeHandler, getByIdHandler, getInProgressHandler);
    }

    [Fact]
    public async Task GetInProgressWorkout_WorkoutExists_ReturnsOkWithDto()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 130m)]);
        _workoutRepo.Setup(x => x.GetInProgressWorkoutAsync(_userId)).ReturnsAsync(workout);

        var result = await _controller.GetInProgressWorkout();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<WorkoutDetailsDto>(okResult.Value);
        Assert.Equal(workout.Id, dto.Id);
        Assert.Equal("InProgress", dto.Status);
        Assert.Single(dto.Exercises);
    }

    [Fact]
    public async Task GetInProgressWorkout_NoWorkoutExists_ReturnsNoContent()
    {
        _workoutRepo.Setup(x => x.GetInProgressWorkoutAsync(_userId)).ReturnsAsync((Workout?)null);

        var result = await _controller.GetInProgressWorkout();

        Assert.IsType<NoContentResult>(result);
    }
}
