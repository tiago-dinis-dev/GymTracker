using Application.Common.Interfaces.Store;
using Common.AI.Models;
using System.Collections.Concurrent;

namespace Infrastructure.AI.Persistence.InMemory;

public class InMemoryExerciseStatsStore : IExerciseStatsStore
{
    private readonly ConcurrentDictionary<(Guid, string), ExerciseStats> _store = new();
    public Task<ExerciseStats?> GetByUserIdAndExerciseNameAsync(Guid userId, string exerciseName, CancellationToken cancellationToken)
    {
        _store.TryGetValue((userId, exerciseName), out var stats);
        return Task.FromResult(stats);
    }

    public Task UpsertAsync(ExerciseStats stats, CancellationToken cancellationToken)
    {
        _store[(stats.UserId, stats.ExerciseName)] = stats;
        return Task.CompletedTask;
    }
}
