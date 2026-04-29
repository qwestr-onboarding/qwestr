using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Application.Abstractions.Persistence;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProfileAndShopItemAsync(Guid profileId, Guid shopItemId, CancellationToken ct = default);
    Task<List<InventoryItem>> GetByProfileAsync(Guid profileId, CancellationToken ct = default);
    Task AddAsync(InventoryItem item, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
