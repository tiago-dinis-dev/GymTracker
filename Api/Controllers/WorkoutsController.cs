using Api.Workouts;
using Application.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/workouts")]
    [ApiController]
    public class WorkoutsController(CreateWorkoutHandler createWorkoutHandler, AddExerciseToWorkoutHandler addExerciseToWorkoutHandler) : ControllerBase
    {
        private readonly CreateWorkoutHandler _createWorkoutHandler = createWorkoutHandler;
        private readonly AddExerciseToWorkoutHandler _addExerciseToWorkoutHandler = addExerciseToWorkoutHandler;

        // GET: api/<WorkoutController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WorkoutController>/5
        [HttpGet("{id}")]
        public string GetWorkoutById(Guid workoutId)
        {
            return "value";
        }

        // POST api/<WorkoutController>
        [HttpPost]
        public async Task<IActionResult> PostWorkout([FromBody] DateTime date)
        {
            CreateWorkoutResult result;
            var command = new CreateWorkoutCommand(Guid.NewGuid(), date);

            try { 
                result = await _createWorkoutHandler.HandleAsync(command);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                nameof(GetWorkoutById),
                new { id = result.WorkoutId },
                new CreateWorkoutResponse(result.WorkoutId)
            );
        }

        [HttpPost("{workoutId}/exercises")]
        public async Task<IActionResult> PostExercise(Guid workoutId, AddExerciseRequest request)
        {
            AddExerciseToWorkoutResult result;
            var command = new AddExerciseToWorkoutCommand(workoutId, request.ExerciseId, [.. request.Sets.Select(s => new Domain.Workouts.SetRecord(s.Reps, s.Weight))]);

            try
            {
                result = await _addExerciseToWorkoutHandler.HandleAsync(command);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

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
