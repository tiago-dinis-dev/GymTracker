using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Workouts;

namespace Application.Workouts;

public record AddExerciseToWorkoutCommand(Guid WorkoutId, Guid ExerciseId, List<SetRecord> Sets);
public record AddExerciseToWorkoutResult(Guid WorkoutId);

public class AddExerciseToWorkoutHandler(IWorkoutRepository workoutRepository, ICacheService cacheService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly ICacheService _cache = cacheService;

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

        await _cache.RemoveAsync(CacheKeys.WorkoutHistory(workout.UserId));
        await _cache.RemoveAsync(CacheKeys.WorkoutDetails(workout.Id));

        await _workoutRepo.SaveChangesAsync();

        return new AddExerciseToWorkoutResult(workout.Id);
    }
}
