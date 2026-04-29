using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly GamificationDbContext _context;

    public InventoryRepository(GamificationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByProfileAndShopItemAsync(Guid profileId, Guid shopItemId, CancellationToken ct = default)
        => await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProfileId == profileId && i.ShopItemId == shopItemId, ct);

    public async Task<List<InventoryItem>> GetByProfileAsync(Guid profileId, CancellationToken ct = default)
        => await _context.InventoryItems.Where(i => i.ProfileId == profileId).ToListAsync(ct);

    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
        => await _context.InventoryItems.AddAsync(item, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
