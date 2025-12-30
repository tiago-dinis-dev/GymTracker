using Application.Dtos;
using Application.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/workout-history")]
public class WorkoutHistoryController(GetWorkoutHistoryHandler getWorkoutHistoryHandler) : ControllerBase
{
    private readonly GetWorkoutHistoryHandler _getWorkoutHistoryHandler = getWorkoutHistoryHandler;

    [HttpGet]
    public async Task<ActionResult<List<WorkoutSummaryDto>>> GetWorkoutHistory(Guid userId)
    {
        var result = await _getWorkoutHistoryHandler.HandleAsync(new GetWorkoutHistoryQuery(userId));

        return Ok(result);
    }

}
