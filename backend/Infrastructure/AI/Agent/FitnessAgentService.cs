#pragma warning disable MAAI001

using Application.AI.Abstractions;
using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Infrastructure.AI.Agent.Skills;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.AI.Agent;

public sealed class FitnessAgentService(
    IWorkoutStatsStore workoutStatsStore,
    IExerciseStatsStore exerciseStatsStore,
    IMuscleGroupStatsStore muscleGroupStatsStore,
    IOptions<AgentOptions> options,
    ILogger<FitnessAgentService> logger) : IFitnessAgentService
{
    private static readonly string AgentInstructions = BuildAgentInstructions();

    public async Task<FitnessInsight> GenerateInsightAsync(Guid userId, CancellationToken ct)
    {
        var swTotal = System.Diagnostics.Stopwatch.StartNew();
        logger.LogInformation("Starting GenerateInsight for user {UserId}", userId);
        var opts = options.Value;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        logger.LogDebug("Building agent (endpoint={Endpoint}, model={Model})", opts.Endpoint, opts.ModelId);

        var openAIClient = new OpenAIClient(
            new ApiKeyCredential(opts.GitHubToken),
            new OpenAIClientOptions
            {
                Endpoint = new Uri(opts.Endpoint),
                NetworkTimeout = TimeSpan.FromMinutes(5),
                RetryPolicy = new ClientRetryPolicy(maxRetries: 0)
            });

        var skillsProvider = new AgentSkillsProvider(
            new WorkoutStatsPlugin(workoutStatsStore),
            new ExerciseStatsPlugin(exerciseStatsStore),
            new MuscleGroupStatsPlugin(muscleGroupStatsStore));

        var agent = openAIClient
            .GetChatClient(opts.ModelId)
            .AsIChatClient()
            .AsAIAgent(new ChatClientAgentOptions
            {
                Name = "FitnessCoach",
                ChatOptions = new ChatOptions { Instructions = AgentInstructions },
                AIContextProviders = [skillsProvider],
            });

        sw.Stop();
        logger.LogInformation("Agent built in {Ms}ms", sw.ElapsedMilliseconds);

        AgentResponse response;
        try
        {
            logger.LogInformation("Invoking agent for user {UserId}", userId);
            sw.Restart();

            response = await agent.RunAsync(
                $"Analyze the fitness data for user {userId}. " +
                "Use all available skills to collect workout stats, exercise stats for common movements, " +
                "and stats for every muscle group. Then produce a comprehensive personalized fitness insight.",
                cancellationToken: ct);

            sw.Stop();
            logger.LogInformation("Agent invocation completed in {Ms}ms", sw.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            logger.LogWarning("GenerateInsight cancelled for user {UserId}", userId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI model invocation failed for user {UserId}: {Message}", userId, ex.Message);
            throw new InvalidOperationException($"AI model invocation failed: {ex.Message}", ex);
        }

        swTotal.Stop();
        logger.LogInformation("GenerateInsight total time for user {UserId}: {Ms}ms", userId, swTotal.ElapsedMilliseconds);

        return ParseInsight(userId, response.Text ?? string.Empty);
    }

    private static string BuildAgentInstructions()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("You are FitnessCoach, a knowledgeable and motivational personal fitness advisor.");
        sb.AppendLine("You have access to skills that retrieve real training data for the user.");
        sb.AppendLine("You must use those skills to gather data before drawing any conclusions.");
        sb.AppendLine();

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(
            "Infrastructure.AI.Agent.Skills.Prompts.PersonalizedAdviceSkill.md");
        if (stream is not null)
        {
            using var reader = new StreamReader(stream);
            sb.AppendLine(reader.ReadToEnd());
        }

        return sb.ToString();
    }

    private static FitnessInsight ParseInsight(Guid userId, string raw)
    {
        try
        {
            var start = raw.IndexOf('{');
            var end = raw.LastIndexOf('}');
            if (start >= 0 && end > start)
            {
                var json = raw[start..(end + 1)];
                var node = JsonNode.Parse(json);
                if (node is not null)
                {
                    var summary = node["summary"]?.GetValue<string>() ?? raw;
                    var findings = node["keyFindings"]?.AsArray()
                        .Select(x => x?.GetValue<string>() ?? string.Empty)
                        .ToList() ?? [];
                    var advice = node["personalizedAdvice"]?.AsArray()
                        .Select(x => x?.GetValue<string>() ?? string.Empty)
                        .ToList() ?? [];

                    return new FitnessInsight(userId, summary, findings, advice, DateTime.UtcNow);
                }
            }
        }
        catch (JsonException) { }

        return new FitnessInsight(userId, raw, [], [], DateTime.UtcNow);
    }
}
