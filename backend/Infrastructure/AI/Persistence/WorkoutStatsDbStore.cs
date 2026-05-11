using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Infrastructure.AI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AI.Persistence;

public class WorkoutStatsDbStore : IWorkoutStatsStore
{
    private readonly AIObservationDbContext _db;

    public WorkoutStatsDbStore(AIObservationDbContext db)
    {
        _db = db;
    }

    public async Task<UserWorkoutStats?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var q = _db.WorkoutCompletedObservations.Where(w => w.UserId == userId);
        if (!await q.AnyAsync(cancellationToken))
            return null;

        var totalWorkouts = await q.CountAsync(cancellationToken);
        var totalVolume = await q.SumAsync(w => w.TotalVolume, cancellationToken);
        var totalExercises = await q.SumAsync(w => w.ExerciseCount, cancellationToken);

        var avgVolume = totalWorkouts > 0 ? totalVolume / totalWorkouts : 0m;

        // Average duration (ticks) then convert
        var avgDurationTicks = await q.AverageAsync(w => (double)w.TotalDuration.Ticks, cancellationToken);
        var avgDuration = TimeSpan.FromTicks(Convert.ToInt64(avgDurationTicks));

        var lastWorkout = await q.MaxAsync(w => w.Timestamp, cancellationToken);

        return new UserWorkoutStats(userId, totalWorkouts, totalExercises, totalVolume, avgVolume, avgDuration, lastWorkout);
    }

    // Upsert is a no-op because stats are derived from persisted observations.
    public Task UpsertAsync(UserWorkoutStats stats, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
