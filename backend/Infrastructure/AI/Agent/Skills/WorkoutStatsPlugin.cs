#pragma warning disable MAAI001

using Application.Common.Interfaces.Store;
using Microsoft.Agents.AI;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.AI.Agent.Skills;

public sealed class WorkoutStatsPlugin(IWorkoutStatsStore store) : AgentClassSkill<WorkoutStatsPlugin>
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

        var stats = await store.GetByUserIdAsync(id, cancellationToken);

        if (stats is null)
            return "No workout stats found for this user.";

        return JsonSerializer.Serialize(stats);
    }

    private static string LoadInstructions(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null) return string.Empty;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
