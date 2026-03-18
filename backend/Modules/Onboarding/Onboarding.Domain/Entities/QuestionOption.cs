using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Вариант ответа для вопроса с выбором.
/// </summary>
public sealed class QuestionOption : Entity<Guid>
{
    private QuestionOption() { }

    public Guid QuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public bool IsCorrect { get; private set; }
    public int Order { get; private set; }

    public Question Question { get; private set; } = null!;

    public static QuestionOption Create(Guid questionId, string text, bool isCorrect, int order)
    {
        return new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = questionId,
            Text = text,
            IsCorrect = isCorrect,
            Order = order
        };
    }

    public void Update(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }

    public void SetOrder(int order) => Order = order;
}
