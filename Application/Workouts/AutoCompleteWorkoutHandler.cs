using Application.Common.Interfaces;

namespace Application.Workouts;

public class AutoCompleteWorkoutHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;

    public async Task HandleAsync(CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddHours(-6);

        var expiredWorkouts = await _workoutRepository.GetExpiredWorkoutsAsync(cutoff, ct);

        foreach (var workout in expiredWorkouts)
        {
            workout.Complete();
        }

        await _workoutRepository.SaveChangesAsync(ct);
    }
}
