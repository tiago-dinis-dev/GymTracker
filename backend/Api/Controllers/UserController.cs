using Application.Dtos;
using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(CreateUserHandler createUserHandler, UpdateUserHandler updateUserHandler, AuthenticateUserHandler authenticateUserHandler, IConfiguration configuration) : ControllerBase
{
    private readonly CreateUserHandler _createUserHandler = createUserHandler;
    private readonly UpdateUserHandler _updateUserHandler = updateUserHandler;
    private readonly AuthenticateUserHandler _authenticateUserHandler = authenticateUserHandler;
    private readonly IConfiguration _configuration = configuration;

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(CreateUserCommand request)
    {
        var user = await _createUserHandler.HandleAsync(request);

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);

        return Ok(new {Token = token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _authenticateUserHandler.HandleAsync(new AuthenticateUserCommand(request.Email, request.Password));

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);

        // Set token in a cookie so same-origin clients (Swagger UI) automatically send it on subsequent requests.
        var cookieOptions = new CookieOptions
        {
            HttpOnly = false, // allow client-side JS if needed; set true if you only want server-side access and adjust JwtBearer accordingly
            Secure = false, // for local dev; set to true in production over HTTPS
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(12)
        };

        Response.Cookies.Append("X-Access-Token", $"Bearer {token}", cookieOptions);

        return Ok(new { Token = token });
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateUser([FromBody] JsonPatchDocument<UpdateUserRequest> patchDocument)
    {
        if (patchDocument == null)
            return BadRequest();

        var dto = new UpdateUserRequest();
        patchDocument.ApplyTo(dto, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new UpdateUserCommand(
            dto.Name,
            dto.Email,
            dto.Weight,
            dto.Height
        );

        await _updateUserHandler.HandleAsync(command);

        return Ok(command);
    }

    private string GenerateJwtToken(UserDto user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("JWT key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
