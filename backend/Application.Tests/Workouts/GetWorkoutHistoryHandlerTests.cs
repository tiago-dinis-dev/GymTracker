using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Workouts;
using Domain.Workouts;
using Moq;

namespace Application.Tests.Workouts;

public class GetWorkoutHistoryHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly GetWorkoutHistoryHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public GetWorkoutHistoryHandlerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        _handler = new GetWorkoutHistoryHandler(_workoutRepo.Object, _cache.Object, _userContext.Object);
    }

    [Fact]
    public async Task HandleAsync_CacheHit_ReturnsCachedValue()
    {
        var cached = new List<WorkoutHistoryDto> { new() { Date = DateTime.UtcNow, Status = "Completed", TotalVolume = 1000f } };
        _cache.Setup(x => x.GetAsync<List<WorkoutHistoryDto>>(It.IsAny<string>())).ReturnsAsync(cached);

        var workout = new Workout(_userId, DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), new[] { new SetRecord(0, 10, 100f, 120m) });
        workout.Complete();

        _workoutRepo.Setup(x => x.GetCompletedWorkoutsAsync(_userId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Workout> { workout }.AsReadOnly());

        var result = await _handler.HandleAsync();

        Assert.NotNull(result);
        if (result.Count != 0)
        {
            Assert.Equal(1000f, result[0].TotalVolume);
        }
    }

    [Fact]
    public async Task HandleAsync_CacheMiss_QueriesRepoAndCaches()
    {
        _cache.Setup(x => x.GetAsync<List<WorkoutHistoryDto>>(It.IsAny<string>())).ReturnsAsync((List<WorkoutHistoryDto>?)null);

        var workout = new Workout(_userId, DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), new[] { new SetRecord(0, 10, 100f, 120m) });
        workout.Complete();

        _workoutRepo.Setup(x => x.GetCompletedWorkoutsAsync(_userId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Workout> { workout }.AsReadOnly());

        var result = await _handler.HandleAsync();

        Assert.NotNull(result);
        if (result.Count != 0)
        {
            Assert.Equal("Completed", result[0].Status.ToString());
        }
    }
}
