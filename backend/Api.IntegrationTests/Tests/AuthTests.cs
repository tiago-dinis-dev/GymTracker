using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.IntegrationTests.Helpers;

namespace Api.IntegrationTests.Tests;

public class AuthTests(GymTrackerFactory factory) : IClassFixture<GymTrackerFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Register_ValidData_Returns200WithToken()
    {
        var client = factory.CreateClient();
        var body = new
        {
            Name = "New User",
            Email = $"register_{Guid.NewGuid():N}@test.com",
            Password = "Pass1234!",
            Weight = 70.0f,
            Height = 175.0f
        };

        var response = await client.PostAsJsonAsync("/api/user/register", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.True(json.TryGetProperty("token", out var token));
        Assert.NotEmpty(token.GetString()!);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsError()
    {
        var client = factory.CreateClient();
        var email = $"dup_{Guid.NewGuid():N}@test.com";
        var body = new { Name = "User", Email = email, Password = "Pass1234!" };

        await client.PostAsJsonAsync("/api/user/register", body);
        var response = await client.PostAsJsonAsync("/api/user/register", body);

        // DomainRuleViolationException is mapped to 400 by ExceptionHandlingMiddleware
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var client = factory.CreateClient();
        var email = $"login_{Guid.NewGuid():N}@test.com";
        var password = "SecurePass99!";

        // Register first
        await client.PostAsJsonAsync("/api/user/register", new { Name = "Login User", Email = email, Password = password });

        // Login
        var response = await client.PostAsJsonAsync("/api/user/login", new { Email = email, Password = password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.True(json.TryGetProperty("token", out _));
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();
        var email = $"badlogin_{Guid.NewGuid():N}@test.com";

        await client.PostAsJsonAsync("/api/user/register", new { Name = "User", Email = email, Password = "RealPass1!" });

        var response = await client.PostAsJsonAsync("/api/user/login", new { Email = email, Password = "WrongPass1!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsync("/api/workouts", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
