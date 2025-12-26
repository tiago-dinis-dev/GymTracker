using Application.Common.Interfaces;
using Application.Exceptions;

namespace Application.Workouts;

public record CompleteWorkoutCommand(Guid WorkoutId);
public record CompleteWorkoutResult(Guid WorkoutId, DateTime CompletedAt);

public class CompleteWorkoutHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;

    public async Task<CompleteWorkoutResult> HandleAsync(CompleteWorkoutCommand command)
    {
        var workout = await _workoutRepository.GetByIdAsync(command.WorkoutId) ?? throw new NotFoundException("Workout not found");

        if (workout.Exercises.Count == 0)
            throw new DomainRuleViolationException("Workout cannot be completed without exercises");

        workout.Complete();

        await _workoutRepository.SaveChangesAsync();
        return new CompleteWorkoutResult(workout.Id, DateTime.UtcNow);
    }
}
