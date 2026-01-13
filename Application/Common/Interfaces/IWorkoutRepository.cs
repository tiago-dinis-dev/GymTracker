using Domain.Workouts;

namespace Application.Common.Interfaces;

public interface IWorkoutRepository
{
    Task AddAsync(Workout workout);
    Task SaveChangesAsync(CancellationToken ct);
    Task<Workout?> GetByIdAsync(Guid workoutId);
    Task<List<Workout>> GetWorkoutsByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Workout>> GetExpiredWorkoutsAsync(DateTime cutoffUtc, CancellationToken ct = default);
}
