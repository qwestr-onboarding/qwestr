using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;

public class ShopItemRepository : IShopItemRepository
{
    private readonly GamificationDbContext _context;

    public ShopItemRepository(GamificationDbContext context)
    {
        _context = context;
    }

    public async Task<ShopItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.ShopItems.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<List<ShopItem>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
        {
            return [];
        }
        return await _context.ShopItems.Where(s => idList.Contains(s.Id)).ToListAsync(ct);
    }

    public async Task<List<ShopItem>> GetByTenantAsync(Guid tenantId, bool activeOnly, CancellationToken ct = default)
    {
        var query = _context.ShopItems.Where(s => s.TenantId == tenantId);
        if (activeOnly)
        {
            query = query.Where(s => s.IsActive);
        }
        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(ShopItem item, CancellationToken ct = default)
        => await _context.ShopItems.AddAsync(item, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
