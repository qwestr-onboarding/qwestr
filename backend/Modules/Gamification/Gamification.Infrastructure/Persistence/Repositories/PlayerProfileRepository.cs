using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;

public class PlayerProfileRepository : IPlayerProfileRepository
{
    private readonly GamificationDbContext _context;

    public PlayerProfileRepository(GamificationDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PlayerProfile?> GetByUserIdAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
        => await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.TenantId == tenantId && p.UserId == userId, ct);

    public async Task<List<PlayerProfile>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.PlayerProfiles.Where(p => p.TenantId == tenantId).ToListAsync(ct);

    public async Task<List<PlayerProfile>> GetTopByXpAsync(Guid tenantId, int topN, CancellationToken ct = default)
        => await _context.PlayerProfiles
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.TotalXp)
            .Take(topN)
            .ToListAsync(ct);

    public async Task<int> GetRankByXpAsync(Guid tenantId, int totalXp, CancellationToken ct = default)
        => await _context.PlayerProfiles.CountAsync(p => p.TenantId == tenantId && p.TotalXp > totalXp, ct) + 1;

    public async Task AddAsync(PlayerProfile profile, CancellationToken ct = default)
        => await _context.PlayerProfiles.AddAsync(profile, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
