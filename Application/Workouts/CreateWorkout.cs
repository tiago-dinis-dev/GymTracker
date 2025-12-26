using Application.Common.Interfaces;
using Domain.Workouts;

namespace Application.Workouts;

public record CreateWorkoutCommand(Guid UserId, DateTime Date);

public record CreateWorkoutResult(Guid WorkoutId);

public class CreateWorkoutHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;

    public async Task<CreateWorkoutResult> HandleAsync(CreateWorkoutCommand command)
    {
        if (command.Date > DateTime.UtcNow)
        {
            throw new ArgumentException("Workout date cannot be in the future.");
        }

        var workout = new Workout(command.UserId, command.Date);

        await _workoutRepo.AddAsync(workout);
        await _workoutRepo.SaveChangesAsync();

        return new CreateWorkoutResult(workout.Id);
    }
}
