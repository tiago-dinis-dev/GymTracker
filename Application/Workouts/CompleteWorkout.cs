using Application.Common.Interfaces;
using Application.Exceptions;

namespace Application.Workouts;

public record CompleteWorkoutCommand(Guid WorkoutId);
public record CompleteWorkoutResult(Guid WorkoutId, DateTime CompletedAt);

public class CompleteWorkoutHandler(IWorkoutRepository workoutRepository, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;
    private readonly IUserContextService _userContextService = userContextService;

    public async Task<CompleteWorkoutResult> HandleAsync(CompleteWorkoutCommand command, CancellationToken ct)
    {
        var userId = _userContextService.GetUserId();

        var workout = await _workoutRepository.GetByIdAsync(command.WorkoutId) ?? throw new NotFoundException("Workout not found");

        if (workout.UserId != userId)
            throw new DomainRuleViolationException("Workout does not belong to user");


        if (workout.Exercises.Count == 0)
            throw new DomainRuleViolationException("Workout cannot be completed without exercises");

        workout.Complete();

        await _workoutRepository.SaveChangesAsync(ct);
        return new CompleteWorkoutResult(workout.Id, DateTime.UtcNow);
    }
}
