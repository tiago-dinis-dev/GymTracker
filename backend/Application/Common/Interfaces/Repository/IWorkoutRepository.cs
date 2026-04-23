using Domain.Workouts;

namespace Application.Common.Interfaces.Repository;

public interface IWorkoutRepository
{
    Task AddAsync(Workout workout);
    Task UpdateAsync(Workout workout);
    Task SaveChangesAsync(CancellationToken ct);
    Task<Workout?> GetWorkoutByIdAsync(Guid workoutId);
    Task<Workout?> GetInProgressWorkoutAsync(Guid userId);
    Task<List<Workout>> GetWorkoutsByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Workout>> GetCompletedWorkoutsAsync(Guid userId, DateTime cutoffUtc, CancellationToken ct = default);
    Task<IReadOnlyList<Workout>> GetExpiredWorkoutsAsync(DateTime cutoffUtc, CancellationToken ct = default);
}
