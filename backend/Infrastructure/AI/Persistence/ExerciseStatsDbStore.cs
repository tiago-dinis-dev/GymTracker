using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Common.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AI.Persistence;

public class ExerciseStatsDbStore(AIObservationDbContext db) : IExerciseStatsStore
{
    public async Task<ExerciseStats?> GetByUserIdAndExerciseNameAsync(Guid userId, string exerciseName, CancellationToken cancellationToken)
    {
        return await db.ExerciseStats
            .Where(e => e.UserId == userId && e.ExerciseName == exerciseName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExerciseStats>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await db.ExerciseStats
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(ExerciseStats stats, CancellationToken cancellationToken)
    {
        var existing = await db.ExerciseStats
            .Where(e => e.UserId == stats.UserId && e.ExerciseName == stats.ExerciseName)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is null)
            db.ExerciseStats.Add(stats);
        else
        {
            db.ExerciseStats.Remove(existing);
            db.ExerciseStats.Add(stats);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
