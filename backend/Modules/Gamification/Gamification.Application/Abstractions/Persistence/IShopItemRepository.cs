using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Application.Abstractions.Persistence;

public interface IShopItemRepository
{
    Task<ShopItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ShopItem>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<List<ShopItem>> GetByTenantAsync(Guid tenantId, bool activeOnly, CancellationToken ct = default);
    Task AddAsync(ShopItem item, CancellationToken ct = default);
    void Delete(ShopItem item);
    Task SaveChangesAsync(CancellationToken ct = default);
}
