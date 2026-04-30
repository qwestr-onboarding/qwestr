using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

/// <summary>
/// Локальная проекция метаданных сценария онбординга:
/// человеко-читаемые названия сценария, этапов и шагов.
/// Обновляется по событиям из модуля Onboarding и используется в UI аналитики
/// и при формировании промпта для LLM, чтобы инсайты содержали понятные названия.
/// Принципиально не хранит самого контента шагов – только идентификаторы и названия.
/// </summary>
public class ScenarioMetadataCache : AggregateRoot<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid ScenarioId { get; private set; }
    public string ScenarioName { get; private set; } = string.Empty;
    public DateTime UpdatedAt { get; private set; }

    private readonly List<StageMetadataItem> _stages = new();
    public IReadOnlyList<StageMetadataItem> Stages => _stages.AsReadOnly();

    // EF Core
    private ScenarioMetadataCache() { }

    public static ActionResult<ScenarioMetadataCache> Create(
        Guid tenantId, Guid scenarioId, string scenarioName,
        IEnumerable<StageMetadataItem> stages, DateTime updatedAt)
    {
        if (tenantId == Guid.Empty || scenarioId == Guid.Empty)
        {
            return ActionResult<ScenarioMetadataCache>.Failure("analytics.metadata.identifiers_required");
        }

        if (string.IsNullOrWhiteSpace(scenarioName))
        {
            return ActionResult<ScenarioMetadataCache>.Failure("analytics.metadata.scenario_name_required");
        }

        var cache = new ScenarioMetadataCache
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ScenarioId = scenarioId,
            ScenarioName = scenarioName.Trim(),
            UpdatedAt = updatedAt
        };

        cache._stages.AddRange(stages ?? Enumerable.Empty<StageMetadataItem>());
        return ActionResult<ScenarioMetadataCache>.Success(cache);
    }

    /// <summary>
    /// Полностью заменить метаданные сценария. Используется при получении
    /// события об изменении структуры сценария — частичные обновления не нужны,
    /// так как Published-сценарий заблокирован, а Draft пересоздаёт структуру целиком.
    /// </summary>
    public ActionResult Replace(string scenarioName, IEnumerable<StageMetadataItem> stages, DateTime updatedAt)
    {
        if (string.IsNullOrWhiteSpace(scenarioName))
        {
            return ActionResult.Failure("analytics.metadata.scenario_name_required");
        }

        ScenarioName = scenarioName.Trim();
        UpdatedAt = updatedAt;
        _stages.Clear();
        _stages.AddRange(stages ?? Enumerable.Empty<StageMetadataItem>());

        return ActionResult.Success();
    }
}

public class StageMetadataItem : Entity<Guid>
{
    public Guid StageId { get; private set; }
    public string StageName { get; private set; } = string.Empty;
    public int Order { get; private set; }

    private readonly List<StepMetadataItem> _steps = new();
    public IReadOnlyList<StepMetadataItem> Steps => _steps.AsReadOnly();

    // EF Core
    private StageMetadataItem() { }

    public StageMetadataItem(Guid stageId, string stageName, int order, IEnumerable<StepMetadataItem> steps)
    {
        Id = Guid.NewGuid();
        StageId = stageId;
        StageName = stageName?.Trim() ?? string.Empty;
        Order = order;
        _steps.AddRange(steps ?? Enumerable.Empty<StepMetadataItem>());
    }
}

public class StepMetadataItem : Entity<Guid>
{
    public Guid StepId { get; private set; }
    public string StepName { get; private set; } = string.Empty;
    public int Order { get; private set; }

    // EF Core
    private StepMetadataItem() { }

    public StepMetadataItem(Guid stepId, string stepName, int order)
    {
        Id = Guid.NewGuid();
        StepId = stepId;
        StepName = stepName?.Trim() ?? string.Empty;
        Order = order;
    }
}