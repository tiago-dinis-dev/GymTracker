#pragma warning disable MAAI001

using Application.Common.Interfaces.Store;
using Microsoft.Agents.AI;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AI.Agent.Skills;

public sealed class WorkoutStatsPlugin(IWorkoutStatsStore store, Infrastructure.AI.Persistence.AIObservationDbContext? dbContext = null) : AgentClassSkill<WorkoutStatsPlugin>
{
    private static readonly string _instructions = LoadInstructions(
        "Infrastructure.AI.Agent.Skills.Prompts.WorkoutSummarySkill.md");

    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "workout-stats",
        "Retrieves aggregated workout statistics for a user: total workouts, total exercises, " +
        "total volume, average volume per workout, average workout duration, and last workout date. " +
        "Use this as the first call to understand the user's overall training activity and consistency.");

    protected override string Instructions => _instructions;

    [AgentSkillScript("get_workout_stats")]
    [Description(
        "Retrieves aggregated workout statistics for a user: total workouts, total exercises, " +
        "total volume, average volume per workout, average workout duration, and last workout date. " +
        "Use this as the first call to understand the user's overall training activity and consistency.")]
    public async Task<string> GetWorkoutStatsAsync(
        [Description("The user's unique identifier (GUID string).")] string userId,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var id))
            return "Invalid userId format.";

        try
        {
            var stats = await store.GetByUserIdAsync(id, cancellationToken);
            if (stats is null)
                return "No workout stats found for this user.";
            return JsonSerializer.Serialize(stats);
        }
        catch (Exception ex)
        {
            return $"Error retrieving workout stats: {ex.Message}";
        }
    }

    [AgentSkillScript("get_recent_workouts")]
    [Description("Returns the most recent completed workouts for a user. Provide userId and optional limit (default 5).")]
    public async Task<string> GetRecentWorkoutsAsync(
        [Description("The user's unique identifier (GUID string).")] string userId,
        [Description("Number of recent workouts to return (optional, default 5).")] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var id))
            return "Invalid userId format.";

        if (dbContext is null)
            return "Observation DB not available in this environment.";

        try
        {
            var q = dbContext.WorkoutCompletedObservations
                .Where(w => w.UserId == id)
                .OrderByDescending(w => w.Timestamp)
                .Take(Math.Max(1, Math.Min(limit, 100)));

            var items = await q.Select(w => new {
                w.WorkoutId,
                w.Timestamp,
                w.TotalVolume,
                w.TotalDuration,
                w.ExerciseCount
            }).ToListAsync(cancellationToken);

            return JsonSerializer.Serialize(items);
        }
        catch (Exception ex)
        {
            return $"Error retrieving recent workouts: {ex.Message}";
        }
    }

    private static string LoadInstructions(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null) return string.Empty;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
