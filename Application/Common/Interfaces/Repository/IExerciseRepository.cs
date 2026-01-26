using Domain.Exercises;

namespace Application.Common.Interfaces.Repository;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(Guid exerciseId);
    Task<List<Exercise>> GetAllAsync();
    Task AddAsync(Exercise exercise);
    Task<bool> ExistsAsync(Guid exerciseId);
}
