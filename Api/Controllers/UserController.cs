using Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(UpdateUserHandler updateUserHandler) : ControllerBase
{
    private readonly UpdateUserHandler _updateUserHandler = updateUserHandler;

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            request.Name,
            request.Email,
            request.Weight,
            request.Height
        );

        await _updateUserHandler.HandleAsync(command);

        return NoContent();
    }

}
