using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Domain.Entities.Insights;

/// <summary>
/// Набор инсайтов и рекомендаций, сгенерированный LLM для одного тенанта в один момент времени.
/// Является корневым агрегатом: гарантирует ограничения на количество элементов
/// и инкапсулирует cooldown-логику ручной генерации.
/// </summary>
public class AnalyticsInsightSet : AggregateRoot<Guid>
{
    public const int MaxInsights = 3;
    public const int MaxRecommendations = 3;

    public Guid TenantId { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public LlmProvider Provider { get; private set; }
    public InsightTrigger Trigger { get; private set; }

    /// <summary>
    /// Хеш входного снимка данных, по которому был сгенерирован набор.
    /// Позволяет при повторной генерации с тем же снимком переиспользовать кешированный результат
    /// и не тратить токены LLM на одно и то же.
    /// </summary>
    public string DataSnapshotHash { get; private set; } = string.Empty;

    private readonly List<Insight> _insights = new();
    public IReadOnlyList<Insight> Insights => _insights.AsReadOnly();

    private readonly List<Recommendation> _recommendations = new();
    public IReadOnlyList<Recommendation> Recommendations => _recommendations.AsReadOnly();

    // EF Core
    private AnalyticsInsightSet() { }

    private AnalyticsInsightSet(
        Guid tenantId, DateTime generatedAt, LlmProvider provider,
        InsightTrigger trigger, string dataSnapshotHash)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        GeneratedAt = generatedAt;
        Provider = provider;
        Trigger = trigger;
        DataSnapshotHash = dataSnapshotHash;
    }

    /// <summary>
    /// Создать набор инсайтов из валидного ответа LLM.
    /// Все элементы валидируются — частично корректный набор не сохраняется.
    /// </summary>
    public static ActionResult<AnalyticsInsightSet> Create(
        Guid tenantId, DateTime generatedAt, LlmProvider provider,
        InsightTrigger trigger, string dataSnapshotHash,
        IReadOnlyList<InsightDraft> insights,
        IReadOnlyList<RecommendationDraft> recommendations)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<AnalyticsInsightSet>.Failure("analytics.insight_set.tenant_required");
        }

        if (string.IsNullOrWhiteSpace(dataSnapshotHash))
        {
            return ActionResult<AnalyticsInsightSet>.Failure("analytics.insight_set.snapshot_hash_required");
        }

        if (insights is null || insights.Count == 0)
        {
            return ActionResult<AnalyticsInsightSet>.Failure("analytics.insight_set.insights_empty");
        }

        if (insights.Count > MaxInsights)
        {
            return ActionResult<AnalyticsInsightSet>.Failure("analytics.insight_set.insights_overflow");
        }

        if (recommendations is not null && recommendations.Count > MaxRecommendations)
        {
            return ActionResult<AnalyticsInsightSet>.Failure("analytics.insight_set.recommendations_overflow");
        }

        foreach (var draft in insights)
        {
            var validation = Insight.Validate(draft.Title, draft.Description, draft.MetricReference);
            if (validation.IsFailure)
            {
                return ActionResult<AnalyticsInsightSet>.Failure(validation.Error!);
            }
        }

        if (recommendations is not null)
        {
            foreach (var draft in recommendations)
            {
                var validation = Recommendation.Validate(draft.Title, draft.Description, draft.Priority);
                if (validation.IsFailure)
                {
                    return ActionResult<AnalyticsInsightSet>.Failure(validation.Error!);
                }
            }
        }

        var set = new AnalyticsInsightSet(tenantId, generatedAt, provider, trigger, dataSnapshotHash);

        foreach (var draft in insights)
        {
            set._insights.Add(new Insight(draft.Title, draft.Description, draft.Severity, draft.MetricReference));
        }

        if (recommendations is not null)
        {
            foreach (var draft in recommendations)
            {
                set._recommendations.Add(new Recommendation(draft.Title, draft.Description, draft.Category, draft.Priority));
            }
        }

        return ActionResult<AnalyticsInsightSet>.Success(set);
    }

    /// <summary>
    /// Проверить, доступна ли ручная генерация для тенанта на основе времени последней Manual-генерации.
    /// </summary>
    public static CooldownStatus EvaluateCooldown(DateTime? lastManualGeneratedAt, DateTime now, TimeSpan cooldown)
    {
        if (lastManualGeneratedAt is null)
        {
            return CooldownStatus.Available;
        }

        var elapsed = now - lastManualGeneratedAt.Value;
        if (elapsed >= cooldown)
        {
            return CooldownStatus.Available;
        }

        var remaining = cooldown - elapsed;
        var availableAt = lastManualGeneratedAt.Value + cooldown;
        return CooldownStatus.OnCooldown(remaining, availableAt);
    }
}

/// <summary>
/// Промежуточное представление инсайта от LLM до доменной валидации.
/// Используется только при создании AnalyticsInsightSet из ответа модели.
/// </summary>
public sealed record InsightDraft(
    string Title,
    string Description,
    InsightSeverity Severity,
    string MetricReference);

/// <summary>
/// Промежуточное представление рекомендации от LLM до доменной валидации.
/// </summary>
public sealed record RecommendationDraft(
    string Title,
    string Description,
    RecommendationCategory Category,
    int Priority);

/// <summary>
/// Статус доступности ручной генерации инсайтов для тенанта.
/// Возвращается из домена в Application-слой и далее в API.
/// </summary>
public readonly record struct CooldownStatus(bool IsAvailable, TimeSpan? Remaining, DateTime? AvailableAt)
{
    public static CooldownStatus Available => new(true, null, null);
    public static CooldownStatus OnCooldown(TimeSpan remaining, DateTime availableAt)
        => new(false, remaining, availableAt);
}