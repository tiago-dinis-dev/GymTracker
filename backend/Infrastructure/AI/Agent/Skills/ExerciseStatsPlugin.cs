#pragma warning disable MAAI001

using Application.Common.Interfaces.Store;
using Microsoft.Agents.AI;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.AI.Agent.Skills;

public sealed class ExerciseStatsPlugin(IExerciseStatsStore store) : AgentClassSkill<ExerciseStatsPlugin>
{
    private static readonly string _instructions = LoadInstructions(
        "Infrastructure.AI.Agent.Skills.Prompts.ExerciseProgressSkill.md");

    public override AgentSkillFrontmatter Frontmatter { get; } = new(
        "exercise-stats",
        "Retrieves performance statistics for a specific exercise performed by a user: total sets, " +
        "total reps, total volume and average weight. Use this to evaluate strength progression, " +
        "stagnation or overtraining on individual movements.");

    protected override string Instructions => _instructions;

    [AgentSkillScript("get_exercise_stats")]
    [Description(
        "Retrieves performance statistics for a specific exercise performed by a user: total sets, " +
        "total reps, total volume and average weight. Use this to evaluate strength progression, " +
        "stagnation or overtraining on individual movements. Call with specific exercise names such as " +
        "'Bench Press', 'Squat', 'Deadlift', 'Pull Up', 'Overhead Press', etc.")]
    public async Task<string> GetExerciseStatsAsync(
        [Description("The user's unique identifier (GUID string).")] string userId,
        [Description("The exact name of the exercise to look up (e.g. 'Bench Press', 'Squat').")] string exerciseName,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var id))
            return "Invalid userId format.";

        var stats = await store.GetByUserIdAndExerciseNameAsync(id, exerciseName, cancellationToken);

        if (stats is null)
            return $"No stats found for exercise '{exerciseName}' for this user.";

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
