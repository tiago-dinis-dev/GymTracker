using Common.AI.Models;

namespace Application.Common.Interfaces.Store;

public interface IExerciseStatsStore
{
    Task<ExerciseStats?> GetByUserIdAndExerciseNameAsync(Guid userId, string exerciseName, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExerciseStats>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpsertAsync(ExerciseStats stats, CancellationToken cancellationToken);
}
