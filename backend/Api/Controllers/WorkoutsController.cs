using Application.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/workouts")]
[Authorize]
[ApiController]
public class WorkoutsController(CreateWorkoutHandler createWorkoutHandler,
    CompleteWorkoutHandler completeWorkoutHandler,
    GetWorkoutByIdHandler getWorkoutByIdHandler,
    GetInProgressWorkoutHandler getInProgressWorkoutHandler) : ControllerBase
{
    private readonly CreateWorkoutHandler _createWorkoutHandler = createWorkoutHandler;
    private readonly CompleteWorkoutHandler _completeWorkoutHandler = completeWorkoutHandler;
    private readonly GetWorkoutByIdHandler _getWorkoutByIdHandler = getWorkoutByIdHandler;
    private readonly GetInProgressWorkoutHandler _getInProgressWorkoutHandler = getInProgressWorkoutHandler;

    #region GET

    [HttpGet("in-progress")]
    public async Task<IActionResult> GetInProgressWorkout()
    {
        var workout = await _getInProgressWorkoutHandler.HandleAsync();
        if (workout is null) return NoContent();
        return Ok(workout);
    }

    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetWorkoutById(Guid workoutId)
    {
        var workout = await _getWorkoutByIdHandler.HandleAsync(new GetWorkoutByIdQuery(workoutId));
        if (workout == null)
            return NotFound();

        return Ok(workout);
    }

    #endregion

    #region POST

    [HttpPost("{workoutId}/complete")]
    public async Task<ActionResult> CompleteWorkout(Guid workoutId, CancellationToken ct)
    {
        await _completeWorkoutHandler.HandleAsync(new CompleteWorkoutCommand(workoutId), ct);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> PostWorkout([FromBody] DateTime date, CancellationToken ct)
    {
        var command = new CreateWorkoutCommand(date);
        var result = await _createWorkoutHandler.HandleAsync(command, ct);

        return CreatedAtAction(
            nameof(GetWorkoutById),
            new { workoutId = result.WorkoutId },
            result
        );
    }

    #endregion

}
