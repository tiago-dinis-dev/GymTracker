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

    public async Task<List<WorkoutSummaryDto>> HandleAsync()
    {
        var userId = _userContext.GetUserId();

        var cacheKey = CacheKeys.WorkoutHistory(userId);
        var cached = await _cache.GetAsync<List<WorkoutSummaryDto>>(cacheKey);

        if (cached != null)
            return cached;

        var workouts = await _workoutRepo.GetWorkoutsByUserIdAsync(userId) ?? new List<Domain.Workouts.Workout>();

        var result = workouts.Select(w => new WorkoutSummaryDto
        {
            WorkoutId = w.Id,
            UserId = w.UserId,
            Date = w.Date,
            Status = w.Status,
            ExerciseCount = w.Exercises.Count,
            TotalVolume = w.CalculateTotalVolume()
        }).ToList();

        await _cache.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30));

        return result;
    }
}
