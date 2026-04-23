using Common.AI.Models;
using Common.Exercises;
using Domain.Exercises;

namespace Application.Common.Interfaces.Store;

public interface IMuscleGroupStatsStore
{
    Task<MuscleGroupStats?> GetByUserAndMuscleGroupAsync(Guid userId, MuscleGroup muscleGroup, CancellationToken ct);
    Task UpsertAsync(MuscleGroupStats stats, CancellationToken ct);
}
