#pragma warning disable MAAI001

using Application.Common.Interfaces.Store;
using Common.Exercises;
using Microsoft.Agents.AI;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.AI.Agent.Skills;

public sealed class MuscleGroupStatsPlugin(IMuscleGroupStatsStore store) : AgentClassSkill<MuscleGroupStatsPlugin>
{
    private static readonly string _instructions = LoadInstructions(
        "Infrastructure.AI.Agent.Skills.Prompts.MuscleGroupBalanceSkill.md");

    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "muscle-group-stats",
        "Retrieves training volume and intensity statistics for a specific muscle group for a user: " +
        "total exercises, total volume, total sets, total reps and average intensity. " +
        "Use this to detect muscle group imbalances and evaluate whether the user trains all areas evenly.");

    protected override string Instructions => _instructions;

    [AgentSkillScript("get_muscle_group_stats")]
    [Description(
        "Retrieves training volume and intensity statistics for a specific muscle group for a user: " +
        "total exercises, total volume, total sets, total reps and average intensity. " +
        "Use this to detect muscle group imbalances and evaluate whether the user trains all areas evenly. " +
        "Valid muscle group values: Chest, Back, Shoulders, Arms, Legs, Abs.")]
    public async Task<string> GetMuscleGroupStatsAsync(
        [Description("The user's unique identifier (GUID string).")] string userId,
        [Description("The muscle group to query. Must be one of: Chest, Back, Shoulders, Arms, Legs, Abs.")] string muscleGroup,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var id))
            return "Invalid userId format.";

        if (!Enum.TryParse<MuscleGroup>(muscleGroup, ignoreCase: true, out var group))
            return $"Invalid muscle group '{muscleGroup}'. Valid values: Chest, Back, Shoulders, Arms, Legs, Abs.";

        var stats = await store.GetByUserAndMuscleGroupAsync(id, group, cancellationToken);

        if (stats is null)
            return $"No stats found for muscle group '{muscleGroup}' for this user.";

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
