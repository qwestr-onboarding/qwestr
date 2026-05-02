using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Abstractions.Persistence;

public interface IAwardRepository
{
    Task<Award?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Award>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<List<Award>> GetActiveByTenantAndTriggerAsync(Guid tenantId, AwardIssuanceType issuanceType, RuleTrigger trigger, CancellationToken ct = default);
    Task<List<EarnedAward>> GetEarnedByProfileAsync(Guid profileId, CancellationToken ct = default);
    Task<bool> IsAwardAlreadyEarnedAsync(Guid profileId, Guid awardId, CancellationToken ct = default);
    Task<AchievementProgress?> GetProgressAsync(Guid profileId, Guid awardId, CancellationToken ct = default);
    Task<List<AchievementProgress>> GetAllProgressByProfileAsync(Guid profileId, CancellationToken ct = default);
    Task AddAsync(Award award, CancellationToken ct = default);
    Task RemoveAsync(Award award, CancellationToken ct = default);
    Task AddEarnedAwardAsync(EarnedAward earnedAward, CancellationToken ct = default);
    Task AddProgressAsync(AchievementProgress progress, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
