using Application.Common.Caching;
using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Exceptions;
using Application.Users;
using Domain.Users;
using Moq;

namespace Application.Tests.Users;

public class CreateUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _handler = new CreateUserHandler(_userRepo.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsUserDto()
    {
        _userRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _handler.HandleAsync(new CreateUserCommand("John", "john@test.com", 80f, 180f));

        Assert.NotNull(result);
        Assert.Equal("John", result!.Name);
        Assert.Equal("john@test.com", result.Email);
        _userRepo.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_Throws()
    {
        var existing = new User(Guid.NewGuid(), "Existing", "john@test.com");
        _userRepo.Setup(x => x.GetByEmailAsync("john@test.com")).ReturnsAsync(existing);

        await Assert.ThrowsAsync<ApplicationDomainRuleViolationException>(() =>
            _handler.HandleAsync(new CreateUserCommand("John", "john@test.com")));
    }
}
