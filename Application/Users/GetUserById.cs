using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Dtos;
using Application.Exceptions;

namespace Application.Users;

public record GetUserByIdQuery(Guid UserId);

public class GetUserByIdHandler(IUserRepository userRepository, ICacheService cacheService)
{
    private readonly IUserRepository _userRepo = userRepository;
    private readonly ICacheService _cache = cacheService;

    public async Task<UserDto?> HandleAsync(GetUserByIdQuery query)
    {
        var cacheKey = CacheKeys.User(query.UserId);
        var cached = await _cache.GetAsync<UserDto>(cacheKey);
        if (cached != null)
            return cached;

        var user = await _userRepo.GetByIdAsync(query.UserId) ?? throw new NotFoundException("User not found.");

        var dto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Weight = user.Weight,
            Height = user.Height
        };

        await _cache.SetAsync(
            cacheKey,
            user,
            TimeSpan.FromMinutes(10)
        );
        
        return dto;
    }
}
