using Qwestr.Modules.Gamification.Domain.ValueObjects;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class PlayerProfile : AggregateRoot<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string? Status { get; private set; }
    public int TotalXp { get; private set; }
    public int AvailableCoins { get; private set; }
    public int CurrentLevel { get; private set; }
    public Equipment Equipment { get; private set; } = Equipment.Empty();
    public DateTime LastActivityAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private PlayerProfile() { } // для EF

    public static PlayerProfile Create(Guid tenantId, Guid userId)
    {
        return new PlayerProfile
        {
            Id = Guid.NewGuid(),
            Status = "",
            TenantId = tenantId,
            UserId = userId,
            TotalXp = 0,
            AvailableCoins = 0,
            CurrentLevel = 1,
            Equipment = Equipment.Empty(),
            LastActivityAt = DateTime.UtcNow
        };
    }

    public void SetStatus(string? status)
    {
        Status = status;
    }
    
    public void AddXp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        TotalXp += amount;
        LastActivityAt = DateTime.UtcNow;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }
        AvailableCoins += amount;
        LastActivityAt = DateTime.UtcNow;
    }

    public ActionResult SpendCoins(int amount)
    {
        if (amount < 0)
        {
            return ActionResult.Failure("PlayerProfile.InvalidAmount");
        }
        if (AvailableCoins < amount)
        {
            return ActionResult.Failure("PlayerProfile.InsufficientCoins");
        }
        AvailableCoins -= amount;
        LastActivityAt = DateTime.UtcNow;
        return ActionResult.Success();
    }

    /// <summary>
    /// Пересчёт уровня на основе TotalXp. Возвращает старый уровень, если изменился.
    /// </summary>
    public int? RecalculateLevel(int newLevel)
    {
        if (newLevel == CurrentLevel)
        {
            return null;
        }
        var oldLevel = CurrentLevel;
        CurrentLevel = newLevel;
        return oldLevel;
    }

    public ActionResult Equip(InventorySlot slot, Guid itemId)
    {
        if (slot == InventorySlot.Consumable)
        {
            return ActionResult.Failure("PlayerProfile.NotEquippable");
        }
        Equipment = Equipment.EquipSlot(slot, itemId);
        return ActionResult.Success();
    }

    public void Unequip(InventorySlot slot)
    {
        Equipment = Equipment.UnequipSlot(slot);
    }
}