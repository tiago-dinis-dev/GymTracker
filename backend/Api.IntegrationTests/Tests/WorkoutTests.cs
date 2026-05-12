using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.IntegrationTests.Helpers;

namespace Api.IntegrationTests.Tests;

public class WorkoutTests(GymTrackerFactory factory) : IClassFixture<GymTrackerFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task CreateWorkout_Authenticated_Returns201WithWorkoutId()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.PostAsync("/api/workouts", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.True(json.TryGetProperty("workoutId", out var workoutId));
        Assert.NotEqual(Guid.Empty, Guid.Parse(workoutId.GetString()!));
    }

    [Fact]
    public async Task CreateWorkout_Unauthenticated_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsync("/api/workouts", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateWorkout_WhenOneInProgress_Returns400()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        // First workout created
        await client.PostAsync("/api/workouts", null);

        // Second attempt should fail
        var response = await client.PostAsync("/api/workouts", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetInProgressWorkout_Authenticated_Returns200WithWorkout()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);
        await client.PostAsync("/api/workouts", null);

        var response = await client.GetAsync("/api/workouts/in-progress");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.True(json.TryGetProperty("id", out _));
    }

    [Fact]
    public async Task GetInProgressWorkout_WhenNone_Returns204()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync("/api/workouts/in-progress");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CompleteWorkout_WithoutExercises_Returns400()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);
        var createResp = await client.PostAsync("/api/workouts", null);
        var createJson = await createResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var workoutId = createJson.GetProperty("workoutId").GetString();

        var response = await client.PostAsync($"/api/workouts/{workoutId}/complete", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CompleteWorkout_WithExercises_Returns200WithResult()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        // Create workout
        var createResp = await client.PostAsync("/api/workouts", null);
        var createJson = await createResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var workoutId = createJson.GetProperty("workoutId").GetString();

        // Get an exercise to add
        var exercisesResp = await client.GetAsync("/api/exercises");
        var exercisesJson = await exercisesResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var exerciseId = exercisesJson[0].GetProperty("id").GetString();

        // Add exercise to workout
        var addExerciseBody = new
        {
            ExerciseId = Guid.Parse(exerciseId!),
            Sets = new[] { new { Index = 0, Reps = 10, Weight = 80.0f, Estimated1Rm = 100m } }
        };
        var addResp = await client.PostAsJsonAsync($"/api/workout-exercises/{workoutId}", addExerciseBody);
        addResp.EnsureSuccessStatusCode();

        // Complete workout
        var response = await client.PostAsync($"/api/workouts/{workoutId}/complete", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.True(json.TryGetProperty("workoutId", out _));
        Assert.True(json.TryGetProperty("completedAt", out _));
    }

    [Fact]
    public async Task CreateWorkout_Within2HourCooldown_Returns400()
    {
        var client = await AuthHelper.CreateAuthenticatedClientAsync(factory);

        // Create and complete a workout
        var createResp = await client.PostAsync("/api/workouts", null);
        var createJson = await createResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var workoutId = createJson.GetProperty("workoutId").GetString();

        var exercisesResp = await client.GetAsync("/api/exercises");
        var exercisesJson = await exercisesResp.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var exerciseId = exercisesJson[0].GetProperty("id").GetString();

        var addExerciseResp = await client.PostAsJsonAsync($"/api/workout-exercises/{workoutId}", new
        {
            ExerciseId = Guid.Parse(exerciseId!),
            Sets = new[] { new { Index = 0, Reps = 8, Weight = 60.0f, Estimated1Rm = 100m } }
        });
        addExerciseResp.EnsureSuccessStatusCode();
        await client.PostAsync($"/api/workouts/{workoutId}/complete", null);

        // Try to create another workout immediately (within 2-hour cooldown)
        var response = await client.PostAsync("/api/workouts", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("2 hours", body);
    }
}
