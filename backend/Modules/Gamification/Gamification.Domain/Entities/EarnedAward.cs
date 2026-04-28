using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class EarnedAward : Entity<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid ProfileId { get; private set; }
    public Guid AwardId { get; private set; }
    public DateTime EarnedAt { get; private set; }
    public Guid? IssuedByUserId { get; private set; }  // только для Manual

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private EarnedAward() { }

    public static EarnedAward CreateAutomatic(Guid tenantId, Guid profileId, Guid awardId)
    {
        return new EarnedAward
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            AwardId = awardId,
            EarnedAt = DateTime.UtcNow,
            IssuedByUserId = null
        };
    }

    public static EarnedAward CreateManual(Guid tenantId, Guid profileId, Guid awardId, Guid issuedByUserId)
    {
        return new EarnedAward
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            AwardId = awardId,
            EarnedAt = DateTime.UtcNow,
            IssuedByUserId = issuedByUserId
        };
    }
}