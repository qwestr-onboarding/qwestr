using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class PointsTransaction : Entity<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid ProfileId { get; private set; }
    public int XpAmount { get; private set; }
    public int CoinsAmount { get; private set; }
    public PointsSourceType SourceType { get; private set; }
    public Guid SourceId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private PointsTransaction() { }

    public static PointsTransaction Create(
        Guid tenantId,
        Guid profileId,
        int xpAmount,
        int coinsAmount,
        PointsSourceType sourceType,
        Guid sourceId,
        string reason)
    {
        return new PointsTransaction
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            XpAmount = xpAmount,
            CoinsAmount = coinsAmount,
            SourceType = sourceType,
            SourceId = sourceId,
            Reason = reason,
            OccurredAt = DateTime.UtcNow
        };
    }
}