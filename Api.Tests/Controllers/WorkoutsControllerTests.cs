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

public class WorkoutsControllerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly WorkoutsController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public WorkoutsControllerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        var createHandler = new CreateWorkoutHandler(_workoutRepo.Object, _cache.Object, _userContext.Object);
        var completeHandler = new CompleteWorkoutHandler(_workoutRepo.Object, _userContext.Object);
        var getByIdHandler = new GetWorkoutByIdHandler(_workoutRepo.Object);
        _controller = new WorkoutsController(createHandler, completeHandler, getByIdHandler);
    }

    [Fact]
    public async Task GetWorkoutById_WorkoutExists_ReturnsOk()
    {
        var workout = new Workout(Guid.NewGuid(), DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 120m)]);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);

        var result = await _controller.GetWorkoutById(workout.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<WorkoutDetailsDto>(okResult.Value);
        Assert.Equal(workout.Id, dto.Id);
    }

    [Fact]
    public async Task CompleteWorkout_ValidWorkout_ReturnsNoContent()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 120m)]);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);

        var result = await _controller.CompleteWorkout(workout.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        _workoutRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
