using Common.AI.Models;

namespace Application.Common.Interfaces.Store;

public interface IWorkoutStatsStore
{
    Task<UserWorkoutStats?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpsertAsync(UserWorkoutStats stats, CancellationToken cancellationToken);
}