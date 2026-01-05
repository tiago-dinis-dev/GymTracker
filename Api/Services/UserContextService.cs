using Application.Common.Interfaces;
using System.Security.Claims;

namespace Api.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedAccessException("User is not authenticated.");

        return Guid.Parse(userIdClaim);
    }
}
