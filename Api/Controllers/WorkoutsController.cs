using Api.Services;
using Application.Workouts;
using Common.Services;
using Common.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/workouts")]
[ApiController]
public class WorkoutsController(CreateWorkoutHandler createWorkoutHandler,
    CompleteWorkoutHandler completeWorkoutHandler, GetWorkoutByIdHandler getWorkoutByIdHandler) : ControllerBase
{
    private readonly CreateWorkoutHandler _createWorkoutHandler = createWorkoutHandler;
    private readonly CompleteWorkoutHandler _completeWorkoutHandler = completeWorkoutHandler;
    private readonly GetWorkoutByIdHandler _getWorkoutByIdHandler = getWorkoutByIdHandler;

    #region GET

    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetWorkoutById(Guid workoutId)
    {
        var workout = _getWorkoutByIdHandler.HandleAsync(new GetWorkoutByIdQuery(workoutId));
        if(workout == null)
            return NotFound();

        return Ok();
    }

    #endregion

    #region POST

    [HttpPost("{workoutId}/complete")]
    public async Task<ActionResult> CompleteWorkout(Guid workoutId)
    {
        await _completeWorkoutHandler.HandleAsync(new CompleteWorkoutCommand(workoutId));

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> PostWorkout([FromBody] DateTime date)
    {
        var command = new CreateWorkoutCommand(date);
        var result = await _createWorkoutHandler.HandleAsync(command);

        return CreatedAtAction(
            nameof(GetWorkoutById),
            new { workoutId = result },
            new CreateWorkoutResponse(result)
        );
    }

    #endregion

    // PUT api/<WorkoutController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<WorkoutController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
