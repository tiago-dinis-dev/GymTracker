using Application.Common.Interfaces;
using Domain.Exercises;

namespace Infrastructure.Persistence.Repositories;

public class ExerciseRepository(GymTrackerDbContext gymTrackerDbContext) : IExerciseRepository
{
    private readonly GymTrackerDbContext _gymTrackerDbContext = gymTrackerDbContext;

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
