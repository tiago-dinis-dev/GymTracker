using Application.Workouts;
using Common.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/exercises")]
[Authorize]
[ApiController]
public class WorkoutExercisesController(AddExerciseToWorkoutHandler addExerciseToWorkoutHandler) : ControllerBase
{
    private readonly AddExerciseToWorkoutHandler _addExerciseToWorkoutHandler = addExerciseToWorkoutHandler;

    [HttpPost("{workoutId}")]
    public async Task<IActionResult> PostExercise(Guid workoutId, [FromBody] AddExerciseRequest request, CancellationToken ct)
    {
        var command = new AddExerciseToWorkoutCommand(workoutId, request.ExerciseId, [.. request.Sets.Select(s => new Domain.Workouts.SetRecord(s.Reps, s.Weight, s.Estimated1Rm))]);

        await _addExerciseToWorkoutHandler.HandleAsync(command, ct);

        return Ok();
    }
}
