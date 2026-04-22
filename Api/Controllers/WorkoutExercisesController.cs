using Application.Workouts;
using Common.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/exercises")]
[Authorize]
[ApiController]
public class WorkoutExercisesController(AddExerciseToWorkoutHandler addExerciseToWorkoutHandler, UpdateExerciseSetInWorkoutHandler updateExerciseInWorkoutHandler) : ControllerBase
{
    private readonly AddExerciseToWorkoutHandler _addExerciseToWorkoutHandler = addExerciseToWorkoutHandler;
    private readonly UpdateExerciseSetInWorkoutHandler _updateExerciseInWorkoutHandler = updateExerciseInWorkoutHandler;

    [HttpPost("{workoutId}")]
    public async Task<IActionResult> PostExercise(Guid workoutId, [FromBody] AddExerciseRequest request, CancellationToken ct)
    {
        if (workoutId == Guid.Empty || request is null || request.ExerciseId == Guid.Empty || request.Sets is null || request.Sets.Count == 0) {
            return BadRequest("Missing required fields.");
        }

        var command = new AddExerciseToWorkoutCommand(workoutId, request.ExerciseId, request.Sets);

        var result = await _addExerciseToWorkoutHandler.HandleAsync(command, ct);

        return Ok(result);
    }

    [HttpPatch("{workoutId}/{exerciseId}")]
    public async Task<IActionResult> UpdateExerciseSet(Guid workoutId, Guid exerciseId, [FromBody] UpdateExerciseSetInWorkoutRequest set, CancellationToken ct)
    {
        if (set is null || set.SetIndex < 0)
        {
            return BadRequest("Set Index must exist or be a non-negative integer.");
        }

        var command = new UpdateExerciseSetInWorkoutCommand(workoutId, exerciseId, set.SetIndex, set.Reps, set.Weight, set.Estimated1Rm);

        var result = await _updateExerciseInWorkoutHandler.HandleAsync(command, ct);

        return Ok(result);
    }
}
