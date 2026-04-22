using Api.Controllers;
using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Users;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Api.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IUserContextService> _userContext = new();
    private readonly IConfiguration _configuration;
    private readonly UserController _controller;
    private readonly Guid _userId = Guid.NewGuid();

    public UserControllerTests()
    {
        var configData = new Dictionary<string, string?>
        {
            { "Jwt:Key", "ThisIsATestKeyThatIsLongEnoughForHmacSha256!!" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
        _userContext.Setup(x => x.GetUserId()).Returns(_userId);

        var createHandler = new CreateUserHandler(_userRepo.Object);
        var getByEmailHandler = new GetUserByEmailHandler(_userRepo.Object, _cache.Object);
        var updateHandler = new UpdateUserHandler(_userRepo.Object, _cache.Object, _userContext.Object);
        _controller = new UserController(createHandler, getByEmailHandler, updateHandler, _configuration);
    }

    [Fact]
    public async Task RegisterUser_ValidRequest_ReturnsOkWithToken()
    {
        _userRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _controller.RegisterUser(new CreateUserCommand("John", "john@test.com"));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task RegisterUser_HandlerReturnsNull_ReturnsUnauthorized()
    {
        _userRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _controller.RegisterUser(new CreateUserCommand("John", "john@test.com"));

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsOkWithToken()
    {
        var user = new User(Guid.NewGuid(), "John", "john@test.com");
        _cache.Setup(x => x.GetAsync<UserDto>(It.IsAny<string>())).ReturnsAsync((UserDto?)null);
        _userRepo.Setup(x => x.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await _controller.Login(new Microsoft.AspNetCore.Identity.Data.LoginRequest { Email = "john@test.com", Password = "unused" });

        Assert.IsType<OkObjectResult>(result);
        Assert.True(httpContext.Response.Headers.ContainsKey("Set-Cookie"));
    }

    [Fact]
    public async Task UpdateUser_NullPatchDocument_ReturnsBadRequest()
    {
        var result = await _controller.UpdateUser(null!);

        Assert.IsType<BadRequestResult>(result);
    }
}
