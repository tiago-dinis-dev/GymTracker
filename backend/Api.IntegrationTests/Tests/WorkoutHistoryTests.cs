using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.IntegrationTests.Helpers;

namespace Api.IntegrationTests.Tests;

public class WorkoutHistoryTests(GymTrackerFactory factory) : IClassFixture<GymTrackerFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetWorkoutHistory_Authenticated_ReturnsOkWithList()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync("/api/workout-history");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.Equal(JsonValueKind.Array, json.ValueKind);
    }

    [Fact]
    public async Task GetWorkoutHistory_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/workout-history");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetWorkoutHistory_AfterCompletedWorkout_ReturnsWorkoutInHistory()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        // Create workout, add exercise, complete it
        var createResp = await client.PostAsync("/api/workouts", null);
        var createJson = await createResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var workoutId = createJson.GetProperty("workoutId").GetString();

        var exercisesResp = await client.GetAsync("/api/exercises");
        var exercisesJson = await exercisesResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var exerciseId = exercisesJson[0].GetProperty("id").GetString();

        var addExResp = await client.PostAsJsonAsync($"/api/workout-exercises/{workoutId}", new
        {
            ExerciseId = Guid.Parse(exerciseId!),
            Sets = new[] { new { Index = 0, Reps = 12, Weight = 50.0f, Estimated1Rm = 100m } }
        });
        addExResp.EnsureSuccessStatusCode();
        await client.PostAsync($"/api/workouts/{workoutId}/complete", null);

        // Check history
        var historyResp = await client.GetAsync("/api/workout-history");
        var history = await historyResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);

        Assert.True(history.GetArrayLength() > 0, "History should contain the completed workout");
    }
}
