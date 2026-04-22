using Api.Controllers;
using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Workouts;
using Common.Exercises;
using Common.Workouts;
using Domain.Exercises;
using Domain.Workouts;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests.Controllers;

public class WorkoutExercisesControllerTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepo = new();
    private readonly Mock<IExerciseRepository> _exerciseRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly WorkoutExercisesController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public WorkoutExercisesControllerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        var addHandler = new AddExerciseToWorkoutHandler(_workoutRepo.Object, _exerciseRepo.Object, _cache.Object, _userContext.Object);
        var updateHandler = new UpdateExerciseSetInWorkoutHandler(_workoutRepo.Object, _exerciseRepo.Object, _userContext.Object);
        _controller = new WorkoutExercisesController(addHandler, updateHandler);
    }

    [Fact]
    public async Task PostExercise_EmptyWorkoutId_ReturnsBadRequest()
    {
        var request = new AddExerciseRequest { ExerciseId = Guid.NewGuid(), Sets = [new SetInfo { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }] };

        var result = await _controller.PostExercise(Guid.Empty, request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostExercise_NullRequest_ReturnsBadRequest()
    {
        var result = await _controller.PostExercise(Guid.NewGuid(), null!, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostExercise_EmptyExerciseId_ReturnsBadRequest()
    {
        var request = new AddExerciseRequest { ExerciseId = Guid.Empty, Sets = [new SetInfo { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m }] };

        var result = await _controller.PostExercise(Guid.NewGuid(), request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostExercise_EmptySets_ReturnsBadRequest()
    {
        var request = new AddExerciseRequest { ExerciseId = Guid.NewGuid(), Sets = [] };

        var result = await _controller.PostExercise(Guid.NewGuid(), request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PostExercise_ValidRequest_ReturnsOkWithResult()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);
        var sets = new List<SetInfo> { new() { Index = 0, Reps = 10, Weight = 100f, Estimated1Rm = 120m } };
        var request = new AddExerciseRequest { ExerciseId = exercise.Id, Sets = sets };

        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        var result = await _controller.PostExercise(workout.Id, request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AddExerciseToWorkoutResult>(okResult.Value);
        Assert.Equal(workout.Id, value.WorkoutId);
        Assert.Equal("Bench Press", value.ExerciseName);
    }

    [Fact]
    public async Task UpdateExerciseSet_NegativeSetIndex_ReturnsBadRequest()
    {
        var set = new UpdateExerciseSetInWorkoutRequest { SetIndex = -1, Reps = 10 };

        var result = await _controller.UpdateExerciseSet(Guid.NewGuid(), Guid.NewGuid(), set, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateExerciseSet_NullRequest_ReturnsBadRequest()
    {
        var result = await _controller.UpdateExerciseSet(Guid.NewGuid(), Guid.NewGuid(), null!, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateExerciseSet_ValidRequest_ReturnsOkWithResult()
    {
        var workout = new Workout(_userId, DateTime.UtcNow);
        var exercise = new Exercise("Bench Press", MuscleGroup.Chest);
        workout.AddExercise(exercise.Id, [new SetRecord(0, 10, 100f, 120m)]);

        _workoutRepo.Setup(x => x.GetWorkoutByIdAsync(workout.Id)).ReturnsAsync(workout);
        _exerciseRepo.Setup(x => x.GetByIdAsync(exercise.Id)).ReturnsAsync(exercise);

        var set = new UpdateExerciseSetInWorkoutRequest { SetIndex = 0, Reps = 12, Weight = 110f, Estimated1Rm = 130m };

        var result = await _controller.UpdateExerciseSet(workout.Id, exercise.Id, set, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<UpdateExerciseSetInWorkoutResult>(okResult.Value);
        Assert.Equal(12, value.Reps);
    }
}
