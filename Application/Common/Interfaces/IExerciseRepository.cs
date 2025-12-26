using Domain.Exercises;

namespace Application.Common.Interfaces;

public interface IExerciseRepository
{
    Task AddAsync(Exercise exercise);
    Task<bool> ExistsAsync(Guid exerciseId);
}
