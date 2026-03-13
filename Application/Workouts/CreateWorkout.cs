using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
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

        await ValidateWorkoutDate(command.Date, userId);

        var workout = new Workout(userId, command.Date);

        await _workoutRepo.AddAsync(workout);

        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(userId));

        await _workoutRepo.SaveChangesAsync(ct);

        return workout.Id;
    }

    private async Task ValidateWorkoutDate(DateTime date, Guid userId)
    {
        var existingWorkouts = await _workoutRepo.GetWorkoutsByUserIdAsync(userId);

        if (date > DateTime.UtcNow)
        {
            throw new ArgumentException("Workout date cannot be in the future.");
        }
        else if(date < DateTime.UtcNow.AddHours(-2))
        {
            throw new ArgumentException("Workout date cannot be more than two hours in the past.");
        }
        else if (existingWorkouts.Any(w => w.Date == date || w.Date < date.Date.AddHours(2)))
        {
            throw new ArgumentException("A workout already exists for the selected date or within the two-hour window.");
        }
    }
}
