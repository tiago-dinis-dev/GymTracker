using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Domain.Common;

namespace Application.Workouts;

public record UpdateExerciseSetInWorkoutCommand(Guid WorkoutId, Guid ExerciseId, int SetIndex, int? Reps = null, float? Weight = null, decimal? Estimated1Rm = null);
public record UpdateExerciseSetInWorkoutResult(Guid WorkoutId, string ExerciseName, int Reps, float Weight, decimal Estimated1Rm, decimal IntensityPercent1Rm);

public class UpdateExerciseSetInWorkoutHandler(IWorkoutRepository workoutRepository, IExerciseRepository exerciseRepository, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepo = workoutRepository;
    private readonly IExerciseRepository _exerciseRepo = exerciseRepository;
    private readonly IUserContextService _userContext = userContextService;

    public async Task<UpdateExerciseSetInWorkoutResult> HandleAsync(UpdateExerciseSetInWorkoutCommand command, CancellationToken ct)
    {
        var userId = _userContext.GetUserId();

        var workout = await _workoutRepo.GetWorkoutByIdAsync(command.WorkoutId) 
            ?? throw new NotFoundException("Workout not found.");

        var exercise = await _exerciseRepo.GetByIdAsync(command.ExerciseId) 
            ?? throw new NotFoundException("Exercise not found.");

        if (workout.UserId != userId)
        {
            throw new ApplicationDomainRuleViolationException("Workout does not belong to user.");
        }

        try
        {
            workout.UpdateExerciseSet(command.ExerciseId, command.SetIndex, command.Reps, command.Weight, command.Estimated1Rm);
        }
        catch (DomainException ex)
        {
            throw new ApplicationDomainRuleViolationException(ex.Message);
        }

        await _workoutRepo.SaveChangesAsync(ct);

        var updatedExercise = workout.Exercises.First(e => e.ExerciseId == command.ExerciseId);
        var updatedSet = updatedExercise.Sets.First(s => s.SetIndex == command.SetIndex);

        return new UpdateExerciseSetInWorkoutResult(
            workout.Id, 
            exercise.Name, 
            updatedSet.Reps, 
            updatedSet.Weight, 
            updatedSet.Estimated1Rm,
            updatedSet.IntensityPercent1Rm
        );
    }
}
