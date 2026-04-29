using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class ShopItem : AggregateRoot<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public InventorySlot Slot { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string IconUrl { get; private set; } = string.Empty;
    public string AssetData { get; private set; } = "{}";  // jsonb с параметрами рендера
    public int Price { get; private set; }
    public int? RequiredLevel { get; private set; }
    public StackingPolicy StackingPolicy { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private ShopItem() { }

    public static ActionResult<ShopItem> Create(
        Guid tenantId, InventorySlot slot, string name, string? description, string iconUrl,
        string assetData, int price, int? requiredLevel, StackingPolicy stackingPolicy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ActionResult<ShopItem>.Failure("ShopItem.EmptyName");
        }
        if (price < 0)
        {
            return ActionResult<ShopItem>.Failure("ShopItem.NegativePrice");
        }
        if (requiredLevel.HasValue && requiredLevel.Value < 0)
        {
            return ActionResult<ShopItem>.Failure("ShopItem.InvalidLevel");
        }
        // бизнес-правило: расходники всегда стакаются
        if (slot == InventorySlot.Consumable && stackingPolicy != StackingPolicy.Stackable)
        {
            return ActionResult<ShopItem>.Failure("ShopItem.ConsumableMustStack");
        }
        return ActionResult<ShopItem>.Success(new ShopItem
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Slot = slot,
            Name = name,
            Description = description,
            IconUrl = iconUrl,
            AssetData = assetData,
            Price = price,
            RequiredLevel = requiredLevel,
            StackingPolicy = stackingPolicy,
            IsActive = true
        });
    }

    public void Update(string name, string? description, string iconUrl, string assetData, int price, int? requiredLevel)
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        AssetData = assetData;
        Price = price;
        RequiredLevel = requiredLevel;
    }

    public void Deactivate() => IsActive = false;

    public ActionResult CanBePurchasedBy(int playerLevel, int playerCoins)
    {
        if (!IsActive)
        {
            return ActionResult.Failure("ShopItem.Inactive");
        }
        if (RequiredLevel.HasValue && playerLevel < RequiredLevel.Value)
        {
            return ActionResult.Failure($"ShopItem.LevelTooLow.{RequiredLevel}");
        }
        if (playerCoins < Price)
        {
            return ActionResult.Failure("ShopItem.InsufficientCoins");
        }
        return ActionResult.Success();
    }
}