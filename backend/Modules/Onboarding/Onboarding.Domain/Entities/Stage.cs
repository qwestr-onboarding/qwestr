using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Логический раздел сценария. Например: «Первый день», «Первая неделя».
/// </summary>
public sealed class Stage : Entity<Guid>
{
    private Stage() { }

    public Guid ScenarioId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public int Order { get; private set; }

    public Scenario Scenario { get; private set; } = null!;
    public ICollection<Step> Steps { get; private set; } = [];

    public static Stage Create(Guid scenarioId, string title, string? description, int order)
    {
        return new Stage
        {
            Id = Guid.NewGuid(),
            ScenarioId = scenarioId,
            Title = title,
            Description = description,
            Order = order
        };
    }

    public void Update(string title, string? description)
    {
        Title = title;
        Description = description;
    }

    public void SetOrder(int order) => Order = order;
}
