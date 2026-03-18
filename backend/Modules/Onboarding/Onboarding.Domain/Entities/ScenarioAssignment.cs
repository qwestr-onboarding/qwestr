using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Назначение сценария сотруднику. Одному сотруднику можно назначить несколько сценариев.
/// TenantId нужен для фильтрации по тенанту без JOIN со Scenario.
/// </summary>
public sealed class ScenarioAssignment : AggregateRoot<Guid>
{
    private ScenarioAssignment() { }

    public Guid ScenarioId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AssignedBy { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public AssignmentStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public ICollection<StepProgress> StepProgresses { get; private set; } = [];

    public static ScenarioAssignment Create(
        Guid scenarioId,
        Guid tenantId,
        Guid userId,
        Guid assignedBy)
    {
        return new ScenarioAssignment
        {
            Id = Guid.NewGuid(),
            ScenarioId = scenarioId,
            TenantId = tenantId,
            UserId = userId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow,
            Status = AssignmentStatus.NotStarted
        };
    }

    public void Start()
    {
        if (Status == AssignmentStatus.NotStarted)
        {
            Status = AssignmentStatus.InProgress;
        }
    }

    public void Complete()
    {
        Status = AssignmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}
