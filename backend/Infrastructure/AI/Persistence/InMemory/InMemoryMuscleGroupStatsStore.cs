using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Common.Exercises;
using Domain.Exercises;
using System.Collections.Concurrent;

namespace Infrastructure.AI.Persistence.InMemory;

public class InMemoryMuscleGroupStatsStore  : IMuscleGroupStatsStore
{
    private readonly ConcurrentDictionary<(Guid, MuscleGroup), MuscleGroupStats> _stats = new();

    public Task<MuscleGroupStats?> GetByUserAndMuscleGroupAsync(Guid userId, MuscleGroup muscleGroup, CancellationToken ct)
    {
        _stats.TryGetValue((userId, muscleGroup), out var stats);
        return Task.FromResult(stats);
    }

    public Task UpsertAsync(MuscleGroupStats stats, CancellationToken ct)
    {
        _stats[(stats.UserId, stats.MuscleGroup)] = stats;
        return Task.CompletedTask;
    }
}
