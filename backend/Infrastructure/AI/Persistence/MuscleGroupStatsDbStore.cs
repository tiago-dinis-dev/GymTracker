using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Common.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AI.Persistence;

public class MuscleGroupStatsDbStore(AIObservationDbContext db) : IMuscleGroupStatsStore
{
    public async Task<MuscleGroupStats?> GetByUserAndMuscleGroupAsync(Guid userId, MuscleGroup muscleGroup, CancellationToken ct)
    {
        return await db.MuscleGroupStats
            .Where(m => m.UserId == userId && m.MuscleGroup == muscleGroup)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<MuscleGroupStats>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await db.MuscleGroupStats
            .Where(m => m.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task UpsertAsync(MuscleGroupStats stats, CancellationToken ct)
    {
        var existing = await db.MuscleGroupStats
            .Where(m => m.UserId == stats.UserId && m.MuscleGroup == stats.MuscleGroup)
            .FirstOrDefaultAsync(ct);

        if (existing is null)
            db.MuscleGroupStats.Add(stats);
        else
        {
            db.MuscleGroupStats.Remove(existing);
            db.MuscleGroupStats.Add(stats);
        }

        await db.SaveChangesAsync(ct);
    }
}
