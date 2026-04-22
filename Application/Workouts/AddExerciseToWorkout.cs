using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Domain.Workouts;
using Domain.Common;
using Common.Workouts;

namespace Application.Workouts;

public record AddExerciseToWorkoutCommand(Guid WorkoutId, Guid ExerciseId, List<SetInfo> Sets);
public record AddExerciseToWorkoutResult(Guid WorkoutId, string ExerciseName, List<SetRecord> Sets);

public class AddExerciseToWorkoutHandler(IWorkoutRepository workoutRepository, IExerciseRepository exerciseRepository, ICacheService cacheService, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly IExerciseRepository _exerciseRepo = exerciseRepository;
    private readonly ICacheService _cache = cacheService;
    private readonly IUserContextService _userContext = userContextService;

    public async Task<AddExerciseToWorkoutResult> HandleAsync(AddExerciseToWorkoutCommand command, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();

        var workout = await _workoutRepo.GetWorkoutByIdAsync(command.WorkoutId) ?? throw new NotFoundException("Workout not found.");
        var exercise = await _exerciseRepo.GetByIdAsync(command.ExerciseId) ?? throw new NotFoundException("Exercise not found.");

        if (workout.UserId != userId)
        {
            throw new ApplicationDomainRuleViolationException("Workout does not belong to user.");
        }

        var sets = command.Sets.Select(s => new SetRecord(s.Index, s.Reps, s.Weight, s.Estimated1Rm)).ToList();

        try
        {
            workout.AddExercise(command.ExerciseId, sets);
        }
        catch (DomainException ex)
        {
            throw new ApplicationDomainRuleViolationException(ex.Message);
        }

        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(userId));
        await _cache.RemoveAsync(CacheKeys.WorkoutDetails(workout.Id));

        await _workoutRepo.SaveChangesAsync(ct);

        var addedExercise = workout.Exercises.First(e => e.ExerciseId == command.ExerciseId);
        var addedSets = addedExercise.Sets;

        return new AddExerciseToWorkoutResult(
            workout.Id, 
            exercise.Name,
            [.. addedSets]
        );
    }
}
