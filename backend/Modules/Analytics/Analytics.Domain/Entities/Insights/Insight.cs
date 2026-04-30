using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Domain.Entities.Insights;

/// <summary>
/// Отдельное наблюдение, входящее в состав AnalyticsInsightSet.
/// Создаётся только через AnalyticsInsightSet.Create – самостоятельный
/// жизненный цикл не имеет.
/// </summary>
public class Insight : Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public InsightSeverity Severity { get; private set; }

    /// <summary>
    /// Ссылка на метрику или сущность, которая стала основанием для инсайта,
    /// в формате понятном UI: например "scenario:{id}", "step:{id}", "engagement".
    /// Используется для подсветки источника при клике на инсайт.
    /// </summary>
    public string MetricReference { get; private set; } = string.Empty;

    // EF Core
    private Insight() { }

    internal Insight(string title, string description, InsightSeverity severity, string metricReference)
    {
        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        Severity = severity;
        MetricReference = metricReference.Trim();
    }

    internal static ActionResult Validate(string title, string description, string metricReference)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 200)
        {
            return ActionResult.Failure("analytics.insight.title_invalid");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Length > 1000)
        {
            return ActionResult.Failure("analytics.insight.description_invalid");
        }

        if (string.IsNullOrWhiteSpace(metricReference) || metricReference.Length > 100)
        {
            return ActionResult.Failure("analytics.insight.metric_reference_invalid");
        }

        return ActionResult.Success();
    }
}