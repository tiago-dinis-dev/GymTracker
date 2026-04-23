using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Common.Security;

namespace Application.Users;

public record AuthenticateUserCommand(string Email, string Password);

public class AuthenticateUserHandler(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepo = userRepository;

    public async Task<UserDto?> HandleAsync(AuthenticateUserCommand command)
    {
        var user = await _userRepo.GetByEmailAsync(command.Email);
        if (user == null) return null;
        if (string.IsNullOrWhiteSpace(user.PasswordHash)) return null;

        if (!PasswordHasher.VerifyHashedPassword(user.PasswordHash, command.Password))
            return null;

        return new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Weight = user.Weight,
            Height = user.Height
        };
    }
}
