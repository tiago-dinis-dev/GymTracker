using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Domain.Workouts;

namespace Application.Workouts;

public record CreateWorkoutCommand(DateTime Date);
public record CreateWorkoutResult(Guid UserId, Guid WorkoutId, DateTime Date, string Status);

public class CreateWorkoutHandler(IWorkoutRepository workoutRepository, ICacheService cacheService, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly ICacheService _cache = cacheService;

    public async Task<CreateWorkoutResult> HandleAsync(CreateWorkoutCommand command, CancellationToken ct)
    {
        var userId = userContextService.GetUserId();
        var existingWorkouts = await _workoutRepo.GetWorkoutsByUserIdAsync(userId);

        await ValiadeIfOnGoingWorkouts(existingWorkouts);
        ValidateCooldown(existingWorkouts);

        var workout = new Workout(userId, command.Date);

        await _workoutRepo.AddAsync(workout);

        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(userId));

        await _workoutRepo.SaveChangesAsync(ct);

        return new CreateWorkoutResult(userId, workout.Id, workout.Date, workout.Status.ToString());
    }

    private static async Task ValidateWorkoutDate(List<Workout> existingWorkouts, DateTime date)
    {
        if (date > DateTime.UtcNow)
        {
            throw new ArgumentException("Workout date cannot be in the future.");
        }
        else if(date < DateTime.UtcNow.AddHours(-2))
        {
            throw new ArgumentException("Workout date cannot be more than two hours in the past.");
        }
        else if (existingWorkouts.Any(w => w.Date == date || w.Date.AddHours(2) > date))
        {
            throw new ArgumentException("A workout already exists for the selected date or within the two-hour window.");
        }
    }

    private static void ValidateCooldown(List<Workout> existingWorkouts)
    {
        if (existingWorkouts.Any(w => w.Status == WorkoutStatus.Completed && w.Date.AddHours(2) > DateTime.UtcNow))
            throw new ArgumentException("You must wait 2 hours before starting a new workout.");
    }

    private static async Task ValiadeIfOnGoingWorkouts(List<Workout> existingWorkouts)
    {
        if (existingWorkouts.Any(w => w.Status == WorkoutStatus.InProgress))
        {
            throw new InvalidOperationException("There is already a workout in progress.");
        }
    }
}
