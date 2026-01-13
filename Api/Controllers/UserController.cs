using Application.Dtos;
using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(CreateUserHandler createUserHandler, GetUserByEmailHandler getUserByEmailHandler, UpdateUserHandler updateUserHandler, IConfiguration configuration) : ControllerBase
{
    private readonly CreateUserHandler _createUserHandler = createUserHandler;
    private readonly GetUserByEmailHandler _getUserByEmailHandler = getUserByEmailHandler;
    private readonly UpdateUserHandler _updateUserHandler = updateUserHandler;
    private readonly IConfiguration _configuration = configuration;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(CreateUserCommand request)
    {
        var user = await _createUserHandler.HandleAsync(new CreateUserCommand(request.Name, request.Email, request.Weight, request.Height));

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);

        return Ok(new {Token = token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _getUserByEmailHandler.HandleAsync(new GetUserByEmailQuery(request.Email));

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    [Authorize]
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

    private string GenerateJwtToken(UserDto user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
