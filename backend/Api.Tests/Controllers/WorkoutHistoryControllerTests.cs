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

public class WorkoutHistoryControllerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly WorkoutHistoryController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public WorkoutHistoryControllerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        var handler = new GetWorkoutHistoryHandler(_workoutRepo.Object, _cache.Object, _userContext.Object);
        _controller = new WorkoutHistoryController(handler);
    }

    [Fact]
    public async Task GetWorkoutHistory_ReturnsOkWithResults()
    {
        var cached = new List<WorkoutSummaryDto>
        {
            new() { WorkoutId = Guid.NewGuid(), UserId = _userId, Date = DateTime.UtcNow, Status = Domain.Workouts.WorkoutStatus.Completed, ExerciseCount = 1, TotalVolume = 1000f }
        };
        _cache.Setup(x => x.GetAsync<List<WorkoutSummaryDto>>(It.IsAny<string>())).ReturnsAsync(cached);

        var result = await _controller.GetWorkoutHistory();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<List<WorkoutSummaryDto>>(okResult.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetWorkoutHistory_EmptyList_ReturnsOk()
    {
        _cache.Setup(x => x.GetAsync<List<WorkoutSummaryDto>>(It.IsAny<string>())).ReturnsAsync((List<WorkoutSummaryDto>?)null);
        _workoutRepo.Setup(x => x.GetCompletedWorkoutsAsync(_userId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Workout>().AsReadOnly());

        var result = await _controller.GetWorkoutHistory();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<List<WorkoutSummaryDto>>(okResult.Value);
        Assert.Empty(value);
    }
}
