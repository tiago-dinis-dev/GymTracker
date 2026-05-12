using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api.IntegrationTests.Helpers;

public static class AuthHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<string> RegisterAndGetTokenAsync(HttpClient client, string? email = null)
    {
        email ??= $"user_{Guid.NewGuid():N}@test.com";

        var body = new
        {
            Name = "Test User",
            Email = email,
            Password = "TestPass123!",
            Weight = 75.0f,
            Height = 180.0f
        };

        var response = await client.PostAsJsonAsync("/api/user/register", body);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        return json.GetProperty("token").GetString()!;
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(GymTrackerFactory factory, string? email = null)
    {
        var client = factory.CreateClient();
        var token = await RegisterAndGetTokenAsync(client, email);
        SetBearerToken(client, token);
        return client;
    }
}
