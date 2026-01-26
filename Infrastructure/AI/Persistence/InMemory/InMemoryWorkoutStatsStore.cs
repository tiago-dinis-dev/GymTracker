using Application.Common.Interfaces.Store;
using Common.AI.Models;
using System.Collections.Concurrent;

namespace Infrastructure.AI.Persistence.InMemory;

public class InMemoryWorkoutStatsStore : IWorkoutStatsStore
{
    private readonly ConcurrentDictionary<Guid, UserWorkoutStats> _stats = new();

    public Task<UserWorkoutStats?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        _stats.TryGetValue(userId, out var stats);
        return Task.FromResult(stats);
    }

    public Task UpsertAsync(UserWorkoutStats stats, CancellationToken cancellationToken)
    {
        _stats[stats.UserId] = stats;
        return Task.CompletedTask;
    }
}
