using Application.Common.Caching;
using Application.Common.Interfaces;
using Domain.Common.Events;

namespace Application.Workouts;

public class WorkoutCompletedCacheHandler(ICacheService cache) : IDomainEventHandler<WorkoutCompleted>
{
    private readonly ICacheService _cache = cache;

    public async Task HandleAsync(WorkoutCompleted domainEvent, CancellationToken cancellationToken =  default)
    {
        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(domainEvent.UserId));
        await _cache.RemoveAsync(CacheKeys.WorkoutDetails(domainEvent.WorkoutId));
    }
}
