using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Dtos;

namespace Application.Workouts;

public class GetWorkoutHistoryHandler(IWorkoutRepository workoutRepository, ICacheService cacheService, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly ICacheService _cache = cacheService;
    private readonly IUserContextService _userContext = userContextService;

    public async Task<List<WorkoutHistoryDto>> HandleAsync()
    {
        var userId = _userContext.GetUserId();

        var cacheKey = CacheKeys.WorkoutHistory(userId);
        var cached = await _cache.GetAsync<List<WorkoutHistoryDto>>(cacheKey);

        if (cached != null)
            return cached;

        var workouts = await _workoutRepo.GetCompletedWorkoutsAsync(userId, DateTime.UtcNow);

        var result = workouts.Select(w => new WorkoutHistoryDto {
            Date = w.Date,
            Status = w.Status.ToString(),
            TotalVolume = w.CalculateTotalVolume()
        }).ToList();

        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromSeconds(30)
        );

        return result;  
    }
}
