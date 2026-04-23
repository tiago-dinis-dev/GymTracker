using Common.AI.Models;

namespace Application.AI.Abstractions;

public interface IFitnessAgentService
{
    Task<FitnessInsight> GenerateInsightAsync(Guid userId, CancellationToken ct);
}
