using Api.Workouts;
using Application.Dtos;
using Application.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/workouts")]
    [ApiController]
    public class WorkoutsController(CreateWorkoutHandler createWorkoutHandler, AddExerciseToWorkoutHandler addExerciseToWorkoutHandler,
        CompleteWorkoutHandler completeWorkoutHandler, GetWorkoutHistoryHandler getWorkoutHistoryHandler, GetWorkoutByIdHandler getWorkoutByIdHandler) : ControllerBase
    {
        private readonly CreateWorkoutHandler _createWorkoutHandler = createWorkoutHandler;
        private readonly AddExerciseToWorkoutHandler _addExerciseToWorkoutHandler = addExerciseToWorkoutHandler;
        private readonly CompleteWorkoutHandler _completeWorkoutHandler = completeWorkoutHandler;
        private readonly GetWorkoutHistoryHandler _getWorkoutHistoryHandler = getWorkoutHistoryHandler;
        private readonly GetWorkoutByIdHandler _getWorkoutByIdHandler = getWorkoutByIdHandler;

        #region GET

        [HttpGet("history")]
        public async Task<ActionResult<List<WorkoutSummaryDto>>> GetWorkoutHistory(Guid userId)
        {
            var result = await _getWorkoutHistoryHandler.HandleAsync(new GetWorkoutHistoryQuery(userId));

            return Ok(result);
        }

        [HttpGet("{id}")]
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
            var command = new CreateWorkoutCommand(Guid.NewGuid(), date);
            var result = await _createWorkoutHandler.HandleAsync(command);

            return CreatedAtAction(
                nameof(GetWorkoutById),
                new { id = result.WorkoutId },
                new CreateWorkoutResponse(result.WorkoutId)
            );
        }

        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> PostExercise(Guid workoutId, AddExerciseRequest request)
        {
            var command = new AddExerciseToWorkoutCommand(workoutId, request.ExerciseId, [.. request.Sets.Select(s => new Domain.Workouts.SetRecord(s.Reps, s.Weight))]);

            var result = await _addExerciseToWorkoutHandler.HandleAsync(command);

            return Ok();
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
}
