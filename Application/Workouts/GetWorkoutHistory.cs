using Application.Common.Interfaces;

namespace Application.Workouts;

public record GetWorkoutHistoryQuery(Guid UserId);
public record WorkoutHistoryItem(Guid WorkoutId, Guid UserId, DateTime Date, string Status, float TotalVolume);

public class GetWorkoutHistoryHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;

    public async Task<List<WorkoutHistoryItem>> HandleAsync(GetWorkoutHistoryQuery query)
    {
        var workouts = await _workoutRepo.GetWorkoutsByUserIdAsync(query.UserId);

        return workouts.Select(w => new WorkoutHistoryItem(
            w.Id,
            w.UserId,
            w.Date,
            w.Status.ToString(),
            w.CalculateTotalVolume()
        )).ToList();
    }
}
