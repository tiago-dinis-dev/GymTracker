using Application.Common.Interfaces.Repository;
using Application.Workouts;
using Domain.Workouts;
using Moq;

namespace Application.Tests.Workouts;

public class AutoCompleteWorkoutHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly AutoCompleteWorkoutHandler _handler;

    public AutoCompleteWorkoutHandlerTests()
    {
        _handler = new AutoCompleteWorkoutHandler(_workoutRepo.Object);
    }

    [Fact]
    public async Task HandleAsync_ExpiredWorkoutsWithExercises_CompletesAll()
    {
        var workout1 = new Workout(Guid.NewGuid(), DateTime.UtcNow.AddHours(-7));
        workout1.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 120m)]);

        var workout2 = new Workout(Guid.NewGuid(), DateTime.UtcNow.AddHours(-8));
        workout2.AddExercise(Guid.NewGuid(), [new SetRecord(0, 8, 80f, 100m)]);

        _workoutRepo.Setup(x => x.GetExpiredWorkoutsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Workout> { workout1, workout2 }.AsReadOnly());

        await _handler.HandleAsync(CancellationToken.None);

        Assert.Equal(WorkoutStatus.Completed, workout1.Status);
        Assert.Equal(WorkoutStatus.Completed, workout2.Status);
        _workoutRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoExpiredWorkouts_DoesNothing()
    {
        _workoutRepo.Setup(x => x.GetExpiredWorkoutsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Workout>().AsReadOnly());

        await _handler.HandleAsync(CancellationToken.None);

        _workoutRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
