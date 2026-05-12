using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.IntegrationTests.Helpers;

namespace Api.IntegrationTests.Tests;

public class ExercisesTests(GymTrackerFactory factory) : IClassFixture<GymTrackerFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetExercises_ReturnsSeededList()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/exercises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0, "Expected seeded exercises to be returned");
    }

    [Fact]
    public async Task GetExercises_EachExerciseHasRequiredFields()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/exercises");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);

        foreach (var exercise in json.EnumerateArray())
        {
            Assert.True(exercise.TryGetProperty("id", out _), "Exercise missing 'id'");
            Assert.True(exercise.TryGetProperty("name", out _), "Exercise missing 'name'");
            Assert.True(exercise.TryGetProperty("muscleGroup", out _), "Exercise missing 'muscleGroup'");
        }
    }
}
