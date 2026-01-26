using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Exceptions;
using Domain.Users;

namespace Application.Users;

public record CreateUserCommand(string Name, string Email, float? Weight = null, float? Height = null);

public record CreateUserResult(Guid UserId);

public class CreateUserHandler(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepo = userRepository;
    public async Task<UserDto?> HandleAsync(CreateUserCommand command)
    {
        var user = new User(Guid.NewGuid(), command.Name, command.Email, command.Weight, command.Height);

        await EnsureUserDoesNotExist(user.UserId);

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        var dto = new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Weight = user.Weight,
            Height = user.Height
        };

        return dto;
    }

    private async Task EnsureUserDoesNotExist(Guid userId)
    {
        var existingUser = await _userRepo.GetByIdAsync(userId);
        if (existingUser != null)
        {
            throw new DomainRuleViolationException("User with this ID already exists.");
        }
    }
}
