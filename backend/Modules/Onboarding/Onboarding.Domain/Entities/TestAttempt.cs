using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Одна попытка прохождения теста. Хранит историю всех попыток по шагу.
/// </summary>
public sealed class TestAttempt : Entity<Guid>
{
    private TestAttempt() { }

    public Guid StepProgressId { get; private set; }
    public int AttemptNumber { get; private set; }
    public int? Score { get; private set; }
    public bool? IsPassed { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public StepProgress StepProgress { get; private set; } = null!;
    public ICollection<QuestionAnswer> QuestionAnswers { get; private set; } = [];

    public static TestAttempt Create(Guid stepProgressId, int attemptNumber)
    {
        return new TestAttempt
        {
            Id = Guid.NewGuid(),
            StepProgressId = stepProgressId,
            AttemptNumber = attemptNumber,
            StartedAt = DateTime.UtcNow
        };
    }

    public void Complete(int score, bool isPassed)
    {
        Score = score;
        IsPassed = isPassed;
        CompletedAt = DateTime.UtcNow;
    }
}
