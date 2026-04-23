namespace Common.AI.Models;

public record FitnessInsight(
    Guid UserId,
    string Summary,
    IReadOnlyList<string> KeyFindings,
    IReadOnlyList<string> PersonalizedAdvice,
    DateTime GeneratedAt
);
