using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Workouts;

namespace Application.Workouts;

public record AddExerciseToWorkoutCommand(Guid WorkoutId, Guid ExerciseId, List<SetRecord> Sets);
public record AddExerciseToWorkoutResult(Guid WorkoutId);

public class AddExerciseToWorkoutHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;

    public async Task<AddExerciseToWorkoutResult> HandleAsync(AddExerciseToWorkoutCommand command)
    {
        var workout = await _workoutRepo.GetByIdAsync(command.WorkoutId) ?? throw new NotFoundException("Workout not found.");
        if (workout.IsCompleted())
            throw new DomainRuleViolationException("Cannot add exercise to a completed workout");

        var exercisePerformed = new ExercisePerformed(command.ExerciseId);

        foreach (var setDto in command.Sets)
        {
            exercisePerformed.AddSet(setDto.Reps, setDto.Weight);
        }

        workout.AddExercise(exercisePerformed);
        await _workoutRepo.SaveChangesAsync();

        return new AddExerciseToWorkoutResult(workout.Id);
    }
}
