using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Application.Users;
using Domain.Users;
using Moq;

namespace Application.Tests.Users;

public class UpdateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly UpdateUserHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public UpdateUserHandlerTests()
    {
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);
        _handler = new UpdateUserHandler(_userRepo.Object, _cache.Object, _userContext.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_UpdatesAndInvalidatesCache()
    {
        var user = new User(_userId, "John", "john@test.com", 80f, 180f);
        _userRepo.Setup(x => x.GetByIdAsync(_userId)).ReturnsAsync(user);

        await _handler.HandleAsync(new UpdateUserCommand(Name: "Jane"));

        Assert.Equal("Jane", user.Name);
        _userRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        _cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UserNotFound_ThrowsNotFoundException()
    {
        _userRepo.Setup(x => x.GetByIdAsync(_userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.HandleAsync(new UpdateUserCommand(Name: "Jane")));
    }
}
