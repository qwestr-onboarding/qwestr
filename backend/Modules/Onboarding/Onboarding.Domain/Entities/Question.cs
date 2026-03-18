using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Вопрос внутри шага. Для шага типа Question — ровно 1 вопрос; для Test — N вопросов.
/// </summary>
public sealed class Question : Entity<Guid>
{
    private Question() { }

    public Guid StepId { get; private set; }
    public QuestionType Type { get; private set; }
    public string QuestionText { get; private set; } = null!;
    public int Order { get; private set; }

    public Step Step { get; private set; } = null!;
    public ICollection<QuestionOption> Options { get; private set; } = [];

    public static Question Create(Guid stepId, QuestionType type, string questionText, int order)
    {
        return new Question
        {
            Id = Guid.NewGuid(),
            StepId = stepId,
            Type = type,
            QuestionText = questionText,
            Order = order
        };
    }

    public void Update(QuestionType type, string questionText)
    {
        Type = type;
        QuestionText = questionText;
    }

    public void SetOrder(int order) => Order = order;
}
