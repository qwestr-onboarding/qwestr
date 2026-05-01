using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Прогресс сотрудника по отдельному шагу.
/// CheckedItems (JSON) — массив отмеченных индексов для шага типа Checklist.
/// </summary>
public sealed class StepProgress : Entity<Guid>
{
    private StepProgress() { }

    public Guid AssignmentId { get; private set; }
    public Guid StepId { get; private set; }
    public StepProgressStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CheckedItems { get; private set; }

    public ScenarioAssignment Assignment { get; private set; } = null!;
    public ICollection<TestAttempt> TestAttempts { get; private set; } = [];
    public ICollection<QuestionAnswer> QuestionAnswers { get; private set; } = [];

    public static StepProgress Create(Guid assignmentId, Guid stepId)
    {
        return new StepProgress
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StepId = stepId,
            Status = StepProgressStatus.NotStarted
        };
    }

    public void Start()
    {
        if (Status is StepProgressStatus.NotStarted or StepProgressStatus.Failed)
        {
            if (!StartedAt.HasValue)
            {
                StartedAt = DateTime.UtcNow;
            }
            Status = StepProgressStatus.InProgress;
        }
    }

    public void Complete()
    {
        Status = StepProgressStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = StepProgressStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    public void UpdateCheckedItems(string checkedItems) => CheckedItems = checkedItems;
}
