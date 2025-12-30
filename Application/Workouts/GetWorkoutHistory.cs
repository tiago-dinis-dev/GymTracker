using Application.Common.Caching;
using Application.Common.Interfaces;

namespace Application.Workouts;

public record GetWorkoutHistoryQuery(Guid UserId);
public record WorkoutHistoryItem(Guid WorkoutId, Guid UserId, DateTime Date, string Status, float TotalVolume);

public class GetWorkoutHistoryHandler(IWorkoutRepository workoutRepository, ICacheService cacheService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly ICacheService _cache = cacheService;

    public async Task<List<WorkoutHistoryItem>> HandleAsync(GetWorkoutHistoryQuery query)
    {
        var cacheKey = CacheKeys.WorkoutHistory(query.UserId);
        var cached = await _cache.GetAsync<List<WorkoutHistoryItem>>(cacheKey);

        if (cached != null)
            return cached;

        var workouts = await _workoutRepo.GetWorkoutsByUserIdAsync(query.UserId);

        var result = workouts.Select(w => new WorkoutHistoryItem(
            w.Id,
            w.UserId,
            w.Date,
            w.Status.ToString(),
            w.CalculateTotalVolume()
        )).ToList();

        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5)
        );

        return result;  
    }
}
