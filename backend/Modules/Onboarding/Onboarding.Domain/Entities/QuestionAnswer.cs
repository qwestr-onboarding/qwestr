using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Ответ сотрудника на один вопрос.
/// Привязывается либо к TestAttempt (шаг типа Test), либо напрямую к StepProgress (шаг типа Question).
/// SelectedOptionIds — список выбранных вариантов (для SingleChoice и MultipleChoice).
/// </summary>
public sealed class QuestionAnswer : Entity<Guid>
{
    private QuestionAnswer() { }

    public Guid? StepProgressId { get; private set; }
    public Guid? TestAttemptId { get; private set; }
    public Guid QuestionId { get; private set; }
    public List<Guid>? SelectedOptionIds { get; private set; }
    public string? TextAnswer { get; private set; }
    public bool IsCorrect { get; private set; }
    public DateTime AnsweredAt { get; private set; }

    public StepProgress? StepProgress { get; private set; }
    public TestAttempt? TestAttempt { get; private set; }

    public static QuestionAnswer CreateForQuestion(
        Guid stepProgressId,
        Guid questionId,
        List<Guid>? selectedOptionIds,
        string? textAnswer,
        bool isCorrect)
    {
        return new QuestionAnswer
        {
            Id = Guid.NewGuid(),
            StepProgressId = stepProgressId,
            QuestionId = questionId,
            SelectedOptionIds = selectedOptionIds,
            TextAnswer = textAnswer,
            IsCorrect = isCorrect,
            AnsweredAt = DateTime.UtcNow
        };
    }

    public static QuestionAnswer CreateForTest(
        Guid testAttemptId,
        Guid questionId,
        List<Guid>? selectedOptionIds,
        string? textAnswer,
        bool isCorrect)
    {
        return new QuestionAnswer
        {
            Id = Guid.NewGuid(),
            TestAttemptId = testAttemptId,
            QuestionId = questionId,
            SelectedOptionIds = selectedOptionIds,
            TextAnswer = textAnswer,
            IsCorrect = isCorrect,
            AnsweredAt = DateTime.UtcNow
        };
    }
}
