using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Application.Workouts;
using Common.Exercises;
using Domain.Exercises;
using Domain.Workouts;
using Moq;

namespace Application.Tests.Workouts;

public class UpdateExerciseSetInWorkoutHandlerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<IExerciseRepository> _exerciseRepo = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly UpdateExerciseSetInWorkoutHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public UpdateExerciseSetInWorkoutHandlerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        _handler = new UpdateExerciseSetInWorkoutHandler(_workoutRepo.Object, _exerciseRepo.Object, _userContext.Object);
    }

    private (Workout workout, Exercise exercise) SetupWorkoutWithExercise()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);
        workout.AddExercise(exercise.Id, [new SetRecord(0, 10, 100f, 120m)]);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);
        return (workout, exercise);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsUpdatedSet()
    {
        var (workout, exercise) = SetupWorkoutWithExercise();

        var result = await _handler.HandleAsync(
            new UpdateExerciseSetInWorkoutCommand(workout.Id, exercise.Id, 0, Reps: 12, Weight: 110f, Estimated1Rm: 130m),
            CancellationToken.None);

        Assert.Equal(12, result.Reps);
        Assert.Equal(110f, result.Weight);
        Assert.Equal(130m, result.Estimated1Rm);
        _workoutRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Workout?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new UpdateExerciseSetInWorkoutCommand(Guid.NewGuid(), Guid.NewGuid(), 0, Reps: 12), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WorkoutBelongsToDifferentUser_Throws()
    {
        var workout = new Workout(Guid.NewGuid(), DateTime.UtcNow);
        var exercise = new Exercise("Squat", MuscleGroup.Legs);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        await Assert.ThrowsAsync<ApplicationDomainRuleViolationException>(() =>
            _handler.HandleAsync(new UpdateExerciseSetInWorkoutCommand(workout.Id, exercise.Id, 0, Reps: 12), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ExerciseNotInWorkout_Throws()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Squat", MuscleGroup.Legs);
        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(new UpdateExerciseSetInWorkoutCommand(workout.Id, exercise.Id, 0, Reps: 12), CancellationToken.None));
    }
}
