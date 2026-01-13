using Application.Dtos;
using Application.Workouts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workout-history")]
public class WorkoutHistoryController(GetWorkoutHistoryHandler getWorkoutHistoryHandler) : ControllerBase
{
    private readonly GetWorkoutHistoryHandler _getWorkoutHistoryHandler = getWorkoutHistoryHandler;

    [HttpGet]
    public async Task<ActionResult<List<WorkoutSummaryDto>>> GetWorkoutHistory()
    {
        var result = await _getWorkoutHistoryHandler.HandleAsync();

        return Ok(result);
    }
}
