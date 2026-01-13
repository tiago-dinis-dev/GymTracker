using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Workouts;

namespace Application.Workouts;

public record AddExerciseToWorkoutCommand(Guid WorkoutId, Guid ExerciseId, List<SetRecord> Sets);
public record AddExerciseToWorkoutResult(Guid WorkoutId);

public class AddExerciseToWorkoutHandler(IWorkoutRepository workoutRepository, ICacheService cacheService, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly ICacheService _cache = cacheService;
    private readonly IUserContextService _userContext = userContextService;

    public async Task<AddExerciseToWorkoutResult> HandleAsync(AddExerciseToWorkoutCommand command, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();

        var workout = await _workoutRepo.GetByIdAsync(command.WorkoutId) ?? throw new NotFoundException("Workout not found.");

        if (workout.UserId != userId)
        {
            throw new DomainRuleViolationException("Workout does not belong to user.");
        }

        workout.AddExercise(command.ExerciseId, command.Sets);

        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(userId));
        await _cache.RemoveAsync(CacheKeys.WorkoutDetails(workout.Id));

        await _workoutRepo.SaveChangesAsync(ct);

        return new AddExerciseToWorkoutResult(workout.Id);
    }
}
