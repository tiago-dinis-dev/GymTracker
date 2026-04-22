using Application.Common.Caching;
using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Exceptions;

namespace Application.Users;

public record GetUserByEmailQuery(string Email);

public class GetUserByEmailHandler(IUserRepository userRepository, ICacheService cacheService)
{
    private readonly IUserRepository _userRepo = userRepository;
    private readonly ICacheService _cache = cacheService;

    public async Task<UserDto?> HandleAsync(GetUserByEmailQuery query)
    {
        var cacheKey = CacheKeys.User(query.Email);
        var cached = await _cache.GetAsync<UserDto>(cacheKey);
        if (cached != null)
            return cached;

        var user = await _userRepo.GetByEmailAsync(query.Email) ?? throw new NotFoundException("User not found.");

        var dto = new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Weight = user.Weight,
            Height = user.Height
        };

        await _cache.SetAsync(
            cacheKey,
            user,
            TimeSpan.FromMinutes(2)
        );
        
        return dto;
    }
}
