namespace Infrastructure.AI.Agent;

public sealed class AgentOptions
{
    public const string SectionName = "FitnessAgent";

    public string GitHubToken { get; set; } = string.Empty;
    public string ModelId { get; set; } = "qwen2.5:7b";
    public string Endpoint { get; set; } = "http://localhost:11434";
}
