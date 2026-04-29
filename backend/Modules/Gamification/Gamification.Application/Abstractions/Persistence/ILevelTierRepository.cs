using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Application.Abstractions.Persistence;

public interface ILevelTierRepository
{
    Task<LevelTier?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<LevelTier>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<LevelTier?> GetForLevelAsync(Guid tenantId, int level, CancellationToken ct = default);
    Task<bool> HasTiersForTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(LevelTier tier, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<LevelTier> tiers, CancellationToken ct = default);
    void Remove(LevelTier tier);
    Task SaveChangesAsync(CancellationToken ct = default);
}
