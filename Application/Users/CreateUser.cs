using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Users;

namespace Application.Users;

public record CreateUserCommand(string Name, string Email, float? Weight = null, float? Height = null);

public record CreateUserResult(Guid UserId);

public class CreateUserHandler(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepo = userRepository;
    public async Task<CreateUserResult> HandleAsync(CreateUserCommand command)
    {
        await EnsureUserDoesNotExist(command.Email);

        var user = new User(command.Name, command.Email, command.Weight, command.Height);
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        return new CreateUserResult(user.Id);
    }

    private async Task EnsureUserDoesNotExist(string email)
    {
        var existingUser = await _userRepo.GetByEmailAsync(email);
        if (existingUser != null)
        {
            throw new DomainRuleViolationException("Email already registered.");
        }
    }
}
