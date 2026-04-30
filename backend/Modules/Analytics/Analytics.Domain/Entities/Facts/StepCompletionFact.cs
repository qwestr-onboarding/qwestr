using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Analytics.Domain.Entities.Facts;

/// <summary>
/// Факт завершения шага сценария онбординга сотрудником.
/// Создаётся в ответ на интеграционное событие из модуля Onboarding и не изменяется после создания.
/// Денормализован намеренно: хранит ScenarioId, StageId и StepType,
/// чтобы все аналитические запросы били в одну таблицу без джойнов.
/// </summary>
public class StepCompletionFact : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid AssignmentId { get; private set; }
    public Guid ScenarioId { get; private set; }
    public Guid StageId { get; private set; }
    public Guid StepId { get; private set; }
    public StepType StepType { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public TimeSpan TimeSpent { get; private set; }
    public bool WasSuccessful { get; private set; }
    public int? Score { get; private set; }
    public int? MaxScore { get; private set; }
    public int AttemptNumber { get; private set; }

    // EF Core
    private StepCompletionFact() { }

    private StepCompletionFact(
        Guid tenantId, Guid assignmentId, Guid scenarioId, Guid stageId,
        Guid stepId, StepType stepType, Guid userId, DateTime completedAt,
        TimeSpan timeSpent, bool wasSuccessful, int? score, int? maxScore, int attemptNumber)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        AssignmentId = assignmentId;
        ScenarioId = scenarioId;
        StageId = stageId;
        StepId = stepId;
        StepType = stepType;
        UserId = userId;
        CompletedAt = completedAt;
        TimeSpent = timeSpent;
        WasSuccessful = wasSuccessful;
        Score = score;
        MaxScore = maxScore;
        AttemptNumber = attemptNumber;
    }

    public static ActionResult<StepCompletionFact> Create(
        Guid tenantId, Guid assignmentId, Guid scenarioId, Guid stageId,
        Guid stepId, StepType stepType, Guid userId, DateTime completedAt,
        TimeSpan timeSpent, bool wasSuccessful, int? score, int? maxScore, int attemptNumber)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<StepCompletionFact>.Failure("analytics.fact.tenant_required");
        }

        if (assignmentId == Guid.Empty || scenarioId == Guid.Empty || stageId == Guid.Empty || stepId == Guid.Empty)
        {
            return ActionResult<StepCompletionFact>.Failure("analytics.fact.identifiers_required");
        }

        if (userId == Guid.Empty)
        {
            return ActionResult<StepCompletionFact>.Failure("analytics.fact.user_required");
        }

        if (timeSpent < TimeSpan.Zero)
        {
            return ActionResult<StepCompletionFact>.Failure("analytics.fact.time_spent_negative");
        }

        if (attemptNumber < 1)
        {
            return ActionResult<StepCompletionFact>.Failure("analytics.fact.attempt_invalid");
        }

        // Score имеет смысл только для тестов и должен быть согласован с MaxScore.
        if (stepType == StepType.Test)
        {
            if (score is null || maxScore is null)
            {
                return ActionResult<StepCompletionFact>.Failure("analytics.fact.test_score_required");
            }

            if (score < 0 || maxScore <= 0 || score > maxScore)
            {
                return ActionResult<StepCompletionFact>.Failure("analytics.fact.score_out_of_range");
            }
        }
        else if (score is not null || maxScore is not null)
        {
            return ActionResult<StepCompletionFact>.Failure("analytics.fact.score_only_for_tests");
        }

        var fact = new StepCompletionFact(
            tenantId, assignmentId, scenarioId, stageId, stepId, stepType,
            userId, completedAt, timeSpent, wasSuccessful, score, maxScore, attemptNumber);

        return ActionResult<StepCompletionFact>.Success(fact);
    }

    /// <summary>
    /// Доля правильных ответов теста. Возвращает null для не-Test шагов.
    /// </summary>
    public decimal? ScoreRatio => Score is not null && MaxScore is > 0
        ? (decimal)Score.Value / MaxScore.Value
        : null;
}