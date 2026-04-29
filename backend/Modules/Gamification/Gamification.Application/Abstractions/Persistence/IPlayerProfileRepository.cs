using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Application.Abstractions.Persistence;

public interface IPlayerProfileRepository
{
    Task<PlayerProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PlayerProfile?> GetByUserIdAsync(Guid tenantId, Guid userId, CancellationToken ct = default);
    Task<List<PlayerProfile>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default);
    Task<List<PlayerProfile>> GetTopByXpAsync(Guid tenantId, int topN, CancellationToken ct = default);
    Task<int> GetRankByXpAsync(Guid tenantId, int totalXp, CancellationToken ct = default);
    Task AddAsync(PlayerProfile profile, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
