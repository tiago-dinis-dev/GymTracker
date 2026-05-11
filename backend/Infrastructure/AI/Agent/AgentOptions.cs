namespace Infrastructure.AI.Agent;

public sealed class AgentOptions
{
    public const string SectionName = "FitnessAgent";

    public string ApiKey { get; set; } = "ollama";
    public string ModelId { get; set; } = "gemma4:31b-cloud";
    public string Endpoint { get; set; } = "http://localhost:11434/v1";
}
