#pragma warning disable MAAI001

using Application.Common.Interfaces.Store;
using Microsoft.Agents.AI;
using System.ComponentModel;
using System.Text.Json;

namespace Infrastructure.AI.Agent.Skills;

public sealed class WorkoutHistoryPlugin(IWorkoutHistoryQuery query) : AgentClassSkill<WorkoutHistoryPlugin>
{
    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "workout-history",
        "Retrieves recent completed workouts directly from the training database, including the full " +
        "exercise list, sets, reps and weights for each session. Always call this alongside the aggregated " +
        "stats skills to give the agent session-level detail for richer and more personalized insights.");

    protected override string Instructions =>
        "Always call get_workout_history as part of every insight generation — not just as a fallback. " +
        "It provides session-level detail (which exercises were done, in which order, with what weights) " +
        "that aggregated stats cannot capture. Use it alongside get_all_exercise_stats and " +
        "get_all_muscle_group_stats: the raw history reveals recent trends and session patterns, while " +
        "the aggregated stats show long-term volume and intensity. Cross-referencing both produces richer, " +
        "more personalized advice. Default to the last 10-20 sessions unless more context is needed.";

    [AgentSkillScript("get_workout_history")]
    [Description(
        "Returns the N most recent completed workouts for a user, including exercise names, muscle groups, " +
        "set count, total reps, total volume and average weight per exercise. " +
        "Use this as a fallback when aggregated stats show no data, or for session-level analysis.")]
    public async Task<string> GetWorkoutHistoryAsync(
        [Description("The user's unique identifier (GUID string).")] string userId,
        [Description("Number of recent workouts to return (default 10, max 50).")] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var id))
            return "Invalid userId format.";

        try
        {
            var history = await query.GetRecentCompletedAsync(id, limit, cancellationToken);
            if (history.Count == 0)
                return "No completed workouts found for this user.";
            return JsonSerializer.Serialize(history);
        }
        catch (Exception ex)
        {
            return $"Error retrieving workout history: {ex.Message}";
        }
    }
}
