using Application.Common.Interfaces;
using Domain.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ExerciseRepository(GymTrackerDbContext gymTrackerDbContext) : IExerciseRepository
{
    private readonly GymTrackerDbContext _gymTrackerDbContext = gymTrackerDbContext;

    public async Task<Exercise?> GetByIdAsync(Guid id)
    {
        return await _gymTrackerDbContext.Exercises
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        return await _gymTrackerDbContext.Exercises
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public Task AddAsync(Exercise exercise)
    {
        _gymTrackerDbContext.Exercises.Add(exercise);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid exerciseId)
    {
        var result = _gymTrackerDbContext.Exercises.Any(e => e.Id == exerciseId);

        return Task.FromResult(result);
    }
}
