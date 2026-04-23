using Application.Common.Caching;
using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Exceptions;
using Application.Users;
using Domain.Users;
using Moq;

namespace Application.Tests.Users;

public class GetUserByEmailHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly GetUserByEmailHandler _handler;

    public GetUserByEmailHandlerTests()
    {
        _handler = new GetUserByEmailHandler(_userRepo.Object, _cache.Object);
    }

    [Fact]
    public async Task HandleAsync_CacheHit_ReturnsCachedValue()
    {
        var cached = new UserDto { UserId = Guid.NewGuid(), Name = "John", Email = "john@test.com" };
        _cache.Setup(x => x.GetAsync<UserDto>(It.IsAny<string>())).ReturnsAsync(cached);

        var result = await _handler.HandleAsync(new GetUserByEmailQuery("john@test.com"));

        Assert.Equal(cached.UserId, result!.UserId);
        _userRepo.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_CacheMiss_QueriesRepoAndCaches()
    {
        _cache.Setup(x => x.GetAsync<UserDto>(It.IsAny<string>())).ReturnsAsync((UserDto?)null);
        var user = new User(Guid.NewGuid(), "John", "john@test.com");
        _userRepo.Setup(x => x.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var result = await _handler.HandleAsync(new GetUserByEmailQuery("john@test.com"));

        Assert.Equal("John", result!.Name);
        _cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ThrowsNotFoundException()
    {
        _cache.Setup(x => x.GetAsync<UserDto>(It.IsAny<string>())).ReturnsAsync((UserDto?)null);
        _userRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new GetUserByEmailQuery("notfound@test.com")));
    }
}
