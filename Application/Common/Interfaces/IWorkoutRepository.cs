using Domain.Workouts;

namespace Application.Common.Interfaces;

public interface IWorkoutRepository
{
    Task AddAsync(Workout workout);
    Task SaveChangesAsync();
    Task<Workout?> GetByIdAsync(Guid workoutId);
    Task<List<Workout>> GetWorkoutsByUserIdAsync(Guid userId);
}
