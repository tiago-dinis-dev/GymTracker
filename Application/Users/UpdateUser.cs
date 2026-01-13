using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Exceptions;

namespace Application.Users;

public record UpdateUserCommand(string? Name = null, string? Email = null, float? Weight = null, float? Height = null);

public record UpdateUserRequest(string? Name = null, string? Email = null, float? Weight = null, float? Height = null);

public class UpdateUserHandler(IUserRepository userRepository, ICacheService cacheService, IUserContextService userContextService)
{
    private readonly IUserRepository _userRepo = userRepository;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IUserContextService _userContextService = userContextService;

    public async Task HandleAsync(UpdateUserCommand command)
    {
        var userId = _userContextService.GetUserId();

        var user = await _userRepo.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        user.UpdateProfile(command.Name, command.Email, command.Weight, command.Height);

        await _userRepo.SaveChangesAsync();
        await _cacheService.RemoveAsync(CacheKeys.User(user.Email));
    }
}
