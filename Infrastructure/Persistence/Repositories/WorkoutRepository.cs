using Application.Common.Interfaces;
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

    public async Task<Workout?> GetByIdAsync(Guid workoutId)
    {
        return await _gymTrackerDbContext.Workouts.FindAsync(workoutId);
    }

    public async Task<List<Workout>> GetWorkoutsByUserIdAsync(Guid userId)
    {
        return await _gymTrackerDbContext.Workouts.Where(w => w.UserId == userId).OrderByDescending(w => w.Date).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _gymTrackerDbContext.SaveChangesAsync();
    }
}
