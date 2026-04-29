using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;

public class LevelTierRepository : ILevelTierRepository
{
    private readonly GamificationDbContext _context;

    public LevelTierRepository(GamificationDbContext context)
    {
        _context = context;
    }

    public async Task<LevelTier?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.LevelTiers.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<List<LevelTier>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.LevelTiers
            .Where(t => t.TenantId == tenantId)
            .OrderBy(t => t.Order)
            .ToListAsync(ct);

    public async Task<LevelTier?> GetForLevelAsync(Guid tenantId, int level, CancellationToken ct = default)
        => await _context.LevelTiers
            .Where(t => t.TenantId == tenantId && t.FromLevel <= level && t.ToLevel >= level)
            .FirstOrDefaultAsync(ct);

    public async Task<bool> HasTiersForTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.LevelTiers.AnyAsync(t => t.TenantId == tenantId, ct);

    public async Task AddAsync(LevelTier tier, CancellationToken ct = default)
        => await _context.LevelTiers.AddAsync(tier, ct);

    public async Task AddRangeAsync(IEnumerable<LevelTier> tiers, CancellationToken ct = default)
        => await _context.LevelTiers.AddRangeAsync(tiers, ct);

    public void Remove(LevelTier tier)
        => _context.LevelTiers.Remove(tier);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
