using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Domain.Entities.Insights;

/// <summary>
/// Конкретное практическое предложение для HR-менеджера, сгенерированное LLM
/// на основе аналитических данных. Жизненный цикл привязан к AnalyticsInsightSet.
/// </summary>
public class Recommendation : Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public RecommendationCategory Category { get; private set; }

    /// <summary>
    /// Приоритет внутри одного набора: 1 — самая важная.
    /// </summary>
    public int Priority { get; private set; }

    // EF Core
    private Recommendation() { }

    internal Recommendation(string title, string description, RecommendationCategory category, int priority)
    {
        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        Category = category;
        Priority = priority;
    }

    internal static ActionResult Validate(string title, string description, int priority)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 200)
        {
            return ActionResult.Failure("analytics.recommendation.title_invalid");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Length > 1000)
        {
            return ActionResult.Failure("analytics.recommendation.description_invalid");
        }

        if (priority < 1 || priority > 10)
        {
            return ActionResult.Failure("analytics.recommendation.priority_out_of_range");
        }

        return ActionResult.Success();
    }
}