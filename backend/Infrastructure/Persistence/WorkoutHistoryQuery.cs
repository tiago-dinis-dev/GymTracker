using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Domain.Workouts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class WorkoutHistoryQuery(GymTrackerDbContext db) : IWorkoutHistoryQuery
{
    public async Task<IReadOnlyList<WorkoutHistorySummary>> GetRecentCompletedAsync(
        Guid userId, int limit, CancellationToken ct)
    {
        var workouts = await db.Workouts
            .Where(w => w.UserId == userId && w.Status == WorkoutStatus.Completed)
            .OrderByDescending(w => w.Date)
            .Take(Math.Clamp(limit, 1, 50))
            .ToListAsync(ct);

        var exerciseIds = workouts
            .SelectMany(w => w.Exercises.Select(e => e.ExerciseId))
            .Distinct()
            .ToList();

        var exerciseMap = await db.Exercises
            .Where(e => exerciseIds.Contains(e.Id))
            .Select(e => new { e.Id, e.Name, MuscleGroup = e.MuscleGroup.ToString() })
            .ToDictionaryAsync(e => e.Id, ct);

        return workouts.Select(w =>
        {
            var exerciseSummaries = w.Exercises.Select(ep =>
            {
                exerciseMap.TryGetValue(ep.ExerciseId, out var meta);
                var sets = ep.Sets.ToList();
                var totalReps = sets.Sum(s => s.Reps);
                var totalVolume = (decimal)sets.Sum(s => s.Weight * s.Reps);
                var avgWeight = totalReps > 0 ? totalVolume / totalReps : 0m;

                return new ExerciseHistorySummary(
                    ExerciseName: meta?.Name ?? ep.ExerciseId.ToString(),
                    MuscleGroup: meta?.MuscleGroup ?? "Unknown",
                    SetCount: sets.Count,
                    TotalReps: totalReps,
                    TotalVolume: totalVolume,
                    AverageWeight: Math.Round(avgWeight, 2)
                );
            }).ToList();

            return new WorkoutHistorySummary(
                WorkoutId: w.Id,
                Date: w.Date,
                ExerciseCount: w.Exercises.Count,
                TotalVolume: (decimal)w.CalculateTotalVolume(),
                Exercises: exerciseSummaries
            );
        }).ToList();
    }
}
