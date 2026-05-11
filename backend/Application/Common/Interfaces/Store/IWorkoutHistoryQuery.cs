using Common.AI.Models;

namespace Application.Common.Interfaces.Store;

public interface IWorkoutHistoryQuery
{
    /// <summary>
    /// Returns the most recent completed workouts for a user with full exercise and set detail.
    /// </summary>
    Task<IReadOnlyList<WorkoutHistorySummary>> GetRecentCompletedAsync(Guid userId, int limit, CancellationToken ct);
}
