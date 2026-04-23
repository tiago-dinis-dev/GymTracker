using Application.AI.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/fitness")]
[Authorize]
[ApiController]
public class FitnessInsightController(IFitnessAgentService agentService) : ControllerBase
{
    [HttpGet("insight/{userId:guid}")]
    public async Task<IActionResult> GetInsight(Guid userId, CancellationToken ct)
    {
        var insight = await agentService.GenerateInsightAsync(userId, ct);
        return Ok(insight);
    }
}
