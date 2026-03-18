using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Минимальная единица прохождения. Один шаг = один экран для сотрудника.
/// Content (JSON): для Information — массив блоков, для Task — описание, для Checklist — пункты.
/// Settings (JSON): для Test — порог, попытки, перемешивание.
/// </summary>
public sealed class Step : Entity<Guid>
{
    private Step() { }

    public Guid StageId { get; private set; }
    public string Title { get; private set; } = null!;
    public StepType Type { get; private set; }
    public int Order { get; private set; }
    public int? Points { get; private set; }
    public string? Content { get; private set; }
    public string? Settings { get; private set; }

    public Stage Stage { get; private set; } = null!;
    public ICollection<Question> Questions { get; private set; } = [];

    public static Step Create(
        Guid stageId,
        string title,
        StepType type,
        int order,
        int? points,
        string? content,
        string? settings)
    {
        return new Step
        {
            Id = Guid.NewGuid(),
            StageId = stageId,
            Title = title,
            Type = type,
            Order = order,
            Points = points,
            Content = content,
            Settings = settings
        };
    }

    public void Update(string title, int? points, string? content, string? settings)
    {
        Title = title;
        Points = points;
        Content = content;
        Settings = settings;
    }

    public void SetOrder(int order) => Order = order;
}
