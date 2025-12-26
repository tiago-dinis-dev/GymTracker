using Application.Common.Interfaces;

namespace Application.Workouts;

public record CompleteWorkoutCommand(Guid WorkoutId);
public record CompleteWorkoutResult(Guid WorkoutId, DateTime CompletedAt);

public class CompleteWorkoutHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;

    public async Task<CompleteWorkoutResult> HandleAsync(CompleteWorkoutCommand command)
    {
        var workout = await _workoutRepository.GetByIdAsync(command.WorkoutId) ?? throw new ArgumentException("Workout not found.");
        workout.Complete();

        await _workoutRepository.SaveChangesAsync();
        return new CompleteWorkoutResult(workout.Id, workout.Date);
    }
}
