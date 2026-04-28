using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class LevelTier : AggregateRoot<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public int Order { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int FromLevel { get; private set; }
    public int ToLevel { get; private set; }
    public string Color { get; private set; } = "#808080";  // hex
    public string? FrameUrl { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private LevelTier() { }

    public static ActionResult<LevelTier> Create(
        Guid tenantId, int order, string title, int fromLevel, int toLevel, string color, string? frameUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return ActionResult<LevelTier>.Failure("LevelTier.EmptyTitle");
        }
        if (fromLevel < 1 || toLevel < fromLevel)
        {
            return ActionResult<LevelTier>.Failure("LevelTier.InvalidRange");
        }
        return ActionResult<LevelTier>.Success(new LevelTier
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Order = order,
            Title = title,
            FromLevel = fromLevel,
            ToLevel = toLevel,
            Color = color,
            FrameUrl = frameUrl
        });
    }

    public void Update(string title, int fromLevel, int toLevel, string color, string? frameUrl)
    {
        Title = title;
        FromLevel = fromLevel;
        ToLevel = toLevel;
        Color = color;
        FrameUrl = frameUrl;
    }
}