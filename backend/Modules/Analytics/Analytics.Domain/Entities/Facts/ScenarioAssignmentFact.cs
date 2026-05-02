using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Analytics.Domain.Entities.Facts;

/// <summary>
/// Факт назначения сценария онбординга сотруднику.
/// В отличие от StepCompletionFact допускает обновление полей жизненного цикла
/// (StartedAt, CompletedAt, CurrentStatus) при поступлении соответствующих событий.
/// Используется для построения воронок прохождения и расчёта retention-когорт.
/// </summary>
public class ScenarioAssignmentFact : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid AssignmentId { get; private set; }
    public Guid ScenarioId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public AssignmentStatus CurrentStatus { get; private set; }

    // EF Core
    private ScenarioAssignmentFact() { }

    private ScenarioAssignmentFact(
        Guid tenantId, Guid assignmentId, Guid scenarioId, Guid userId, DateTime assignedAt)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        AssignmentId = assignmentId;
        ScenarioId = scenarioId;
        UserId = userId;
        AssignedAt = assignedAt;
        CurrentStatus = AssignmentStatus.NotStarted;
    }

    public static ActionResult<ScenarioAssignmentFact> Create(
        Guid tenantId, Guid assignmentId, Guid scenarioId, Guid userId, DateTime assignedAt)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<ScenarioAssignmentFact>.Failure("analytics.assignment.tenant_required");
        }

        if (assignmentId == Guid.Empty || scenarioId == Guid.Empty || userId == Guid.Empty)
        {
            return ActionResult<ScenarioAssignmentFact>.Failure("analytics.assignment.identifiers_required");
        }

        var fact = new ScenarioAssignmentFact(tenantId, assignmentId, scenarioId, userId, assignedAt);
        return ActionResult<ScenarioAssignmentFact>.Success(fact);
    }

    /// <summary>
    /// Отметить, что сотрудник начал прохождение сценария (выполнил первый шаг).
    /// Идемпотентно: повторный вызов не сдвигает дату старта.
    /// </summary>
    public ActionResult MarkStarted(DateTime startedAt)
    {
        if (StartedAt is not null)
        {
            return ActionResult.Success();
        }

        if (startedAt < AssignedAt)
        {
            return ActionResult.Failure("analytics.assignment.started_before_assigned");
        }

        StartedAt = startedAt;
        CurrentStatus = AssignmentStatus.InProgress;
        return ActionResult.Success();
    }

    public ActionResult MarkCompleted(DateTime completedAt)
    {
        if (StartedAt is null)
        {
            StartedAt = AssignedAt;
        }

        if (completedAt < StartedAt)
        {
            return ActionResult.Failure("analytics.assignment.completed_before_started");
        }

        CompletedAt = completedAt;
        CurrentStatus = AssignmentStatus.Completed;
        return ActionResult.Success();
    }

    /// <summary>
    /// Длительность прохождения сценария: от первого выполненного шага до завершения.
    /// Null, если сценарий ещё не завершён.
    /// </summary>
    public TimeSpan? Duration => StartedAt is not null && CompletedAt is not null
        ? CompletedAt.Value - StartedAt.Value
        : null;
}