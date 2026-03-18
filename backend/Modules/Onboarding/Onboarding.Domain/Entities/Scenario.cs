using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

/// <summary>
/// Программа адаптации, принадлежащая компании (Tenant).
/// </summary>
public sealed class Scenario : AggregateRoot<Guid>, IAuditableEntity
{
    private Scenario() { }

    public Guid TenantId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public List<string> Tags { get; private set; } = [];
    public bool IsPublished { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Stage> Stages { get; private set; } = [];

    public static Scenario Create(Guid tenantId, string title, string? description, List<string> tags)
    {
        return new Scenario
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = title,
            Description = description,
            Tags = tags,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string? description, List<string> tags)
    {
        Title = title;
        Description = description;
        Tags = tags;
    }

    public void Publish() => IsPublished = true;

    public void Unpublish() => IsPublished = false;
}
