using Application.Common.Caching;
using Application.Common.Interfaces;
using Domain.Workouts;

namespace Application.Workouts;

public record CreateWorkoutCommand(DateTime Date);

public class CreateWorkoutHandler(IWorkoutRepository workoutRepository, ICacheService cacheService, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly ICacheService _cache = cacheService;

    public async Task<Guid> HandleAsync(CreateWorkoutCommand command, CancellationToken ct)
    {
        var userId = userContextService.GetUserId();

        if (command.Date > DateTime.UtcNow)
        {
            throw new ArgumentException("Workout date cannot be in the future.");
        }

        var workout = new Workout(userId, command.Date);

        await _workoutRepo.AddAsync(workout);

        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(userId));

        await _workoutRepo.SaveChangesAsync(ct);

        return workout.Id;
    }
}
