using Application.Common.Interfaces.Repository;
using Domain.Workouts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class WorkoutRepository(GymTrackerDbContext gymTrackerDbContext) : IWorkoutRepository
{
    private readonly GymTrackerDbContext _gymTrackerDbContext = gymTrackerDbContext;

    public Task AddAsync(Workout workout)
    {
        _gymTrackerDbContext.Workouts.Add(workout);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Workout workout)
    {
        _gymTrackerDbContext.Workouts.Update(workout);

        return Task.CompletedTask;
    }

    public async Task<Workout?> GetInProgressWorkoutAsync(Guid userId)
    {
        return await _gymTrackerDbContext.Workouts
            .FirstOrDefaultAsync(w => w.UserId == userId && w.Status == WorkoutStatus.InProgress);
    }

    public async Task<Workout?> GetWorkoutByIdAsync(Guid workoutId)
    {
        return await _gymTrackerDbContext.Workouts.FindAsync(workoutId);
    }

    public async Task<List<Workout>> GetWorkoutsByUserIdAsync(Guid userId)
    {
        return await _gymTrackerDbContext.Workouts.Where(w => w.UserId == userId).OrderByDescending(w => w.Date).ToListAsync();
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _gymTrackerDbContext.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Workout>> GetCompletedWorkoutsAsync(Guid userId, DateTime cutoffUtc, CancellationToken ct = default)
    {
        return await _gymTrackerDbContext.Workouts
            .Where(w => w.UserId == userId && w.Status == WorkoutStatus.Completed && w.Date <= cutoffUtc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Workout>> GetExpiredWorkoutsAsync(DateTime cutoffUtc, CancellationToken ct = default)
    {
        return await _gymTrackerDbContext.Workouts
            .Where(w => w.Status != WorkoutStatus.Completed && w.Date <= cutoffUtc)
            .ToListAsync(ct);
    }
}
