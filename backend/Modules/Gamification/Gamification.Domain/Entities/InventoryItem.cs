using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class InventoryItem : Entity<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid ProfileId { get; private set; }
    public Guid ShopItemId { get; private set; }
    public int Quantity { get; private set; }
    public InventoryAcquisitionSource Source { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private InventoryItem() { }

    public static InventoryItem Create(
        Guid tenantId, Guid profileId, Guid shopItemId, int quantity, InventoryAcquisitionSource source)
    {
        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProfileId = profileId,
            ShopItemId = shopItemId,
            Quantity = quantity,
            Source = source,
            AcquiredAt = DateTime.UtcNow
        };
    }

    public void IncreaseQuantity(int delta)
    {
        if (delta <= 0)
        {
            return;
        }
        Quantity += delta;
    }

    public ActionResult DecreaseQuantity(int delta)
    {
        if (delta <= 0)
        {
            return ActionResult.Failure("InventoryItem.InvalidDelta");
        }
        if (Quantity < delta)
        {
            return ActionResult.Failure("InventoryItem.InsufficientQuantity");
        }
        Quantity -= delta;
        return ActionResult.Success();
    }
}