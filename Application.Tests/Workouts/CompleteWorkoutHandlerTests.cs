using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Application.Workouts;
using Domain.Workouts;
using Moq;

namespace Application.Tests.Workouts;

public class CompleteWorkoutHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly CompleteWorkoutHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public CompleteWorkoutHandlerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        _handler = new CompleteWorkoutHandler(_workoutRepo.Object, _userContext.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidWorkout_CompletesSuccessfully()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        workout.AddExercise(Guid.NewGuid(), [new SetRecord(0, 10, 100f, 120m)]);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);

        var result = await _handler.HandleAsync(new CompleteWorkoutCommand(workout.Id), CancellationToken.None);

        Assert.Equal(workout.Id, result.WorkoutId);
        Assert.Equal(WorkoutStatus.Completed, workout.Status);
        _workoutRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Workout?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new CompleteWorkoutCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WorkoutBelongsToDifferentUser_Throws()
    {
        var workout = new Workout(Guid.NewGuid(), DateTime.UtcNow);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);

        await Assert.ThrowsAsync<ApplicationDomainRuleViolationException>(() =>
            _handler.HandleAsync(new CompleteWorkoutCommand(workout.Id), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WorkoutWithNoExercises_Throws()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);

        await Assert.ThrowsAsync<ApplicationDomainRuleViolationException>(() =>
            _handler.HandleAsync(new CompleteWorkoutCommand(workout.Id), CancellationToken.None));
    }
}
