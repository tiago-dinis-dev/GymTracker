using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Application.Workouts;
using Common.Workouts;
using Domain.Exercises;
using Domain.Workouts;
using Common.Exercises;
using Moq;

namespace Application.Tests.Workouts;

public class AddExerciseToWorkoutHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<IExerciseRepository> _exerciseRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly AddExerciseToWorkoutHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public AddExerciseToWorkoutHandlerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        _handler = new AddExerciseToWorkoutHandler(_workoutRepo.Object, _exerciseRepo.Object, _cache.Object, _userContext.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsResult()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);
        var sets = new List<SetInfo> { new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m } };

        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        var result = await _handler.HandleAsync(new AddExerciseToWorkoutCommand(workout.Id, exercise.Id, sets), CancellationToken.None);

        Assert.Equal(workout.Id, result.WorkoutId);
        Assert.Equal("Bench Press", result.ExerciseName);
        Assert.Single(result.Sets);
        _workoutRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Workout?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new AddExerciseToWorkoutCommand(Guid.NewGuid(), Guid.NewGuid(), [new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }]), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ExerciseNotFound_ThrowsNotFoundException()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Exercise?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new AddExerciseToWorkoutCommand(workout.Id, Guid.NewGuid(), [new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }]), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WorkoutBelongsToDifferentUser_ThrowsApplicationDomainRuleViolation()
    {
        var otherUserId = Guid.NewGuid();
        var workout = new Workout(otherUserId, DateTime.UtcNow);
        var exercise = new Exercise("Squat", MuscleGroup.Legs);

        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        await Assert.ThrowsAsync<ApplicationDomainRuleViolationException>(() =>
            _handler.HandleAsync(new AddExerciseToWorkoutCommand(workout.Id, exercise.Id, [new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }]), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_DuplicateExercise_ThrowsInvalidOperation()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);
        var sets = new List<SetInfo> { new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m } };

        workout.AddExercise(exercise.Id, [new SetRecord(0, 10, 100f, 120m)]);

        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(new AddExerciseToWorkoutCommand(workout.Id, exercise.Id, sets), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_InvalidatesCache()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);
        var sets = new List<SetInfo> { new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m } };

        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        await _handler.HandleAsync(new AddExerciseToWorkoutCommand(workout.Id, exercise.Id, sets), CancellationToken.None);

        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Exactly(2));
    }
}
