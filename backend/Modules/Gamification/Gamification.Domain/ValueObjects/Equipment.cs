using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Domain.ValueObjects;

public sealed class Equipment
{
    public Guid? ProfileColorItemId { get; private set; }
    public Guid? AvatarFrameItemId { get; private set; }
    public Guid? BackgroundItemId { get; private set; }
    public Guid? PetItemId { get; private set; }
    public Guid? ClassItemId { get; private set; }
    public Guid? WeaponItemId { get; private set; }

    private Equipment() { }

    private Equipment(
        Guid? profileColor,
        Guid? avatarFrame,
        Guid? background,
        Guid? pet,
        Guid? @class,
        Guid? weapon)
    {
        ProfileColorItemId = profileColor;
        AvatarFrameItemId = avatarFrame;
        BackgroundItemId = background;
        PetItemId = pet;
        ClassItemId = @class;
        WeaponItemId = weapon;
    }

    public static Equipment Empty() => new(null, null, null, null, null, null);

    public Equipment EquipSlot(InventorySlot slot, Guid itemId)
    {
        return slot switch
        {
            InventorySlot.ProfileColor => new Equipment(itemId, AvatarFrameItemId, BackgroundItemId, PetItemId, ClassItemId, WeaponItemId),
            InventorySlot.AvatarFrame  => new Equipment(ProfileColorItemId, itemId, BackgroundItemId, PetItemId, ClassItemId, WeaponItemId),
            InventorySlot.Background   => new Equipment(ProfileColorItemId, AvatarFrameItemId, itemId, PetItemId, ClassItemId, WeaponItemId),
            InventorySlot.Pet          => new Equipment(ProfileColorItemId, AvatarFrameItemId, BackgroundItemId, itemId, ClassItemId, WeaponItemId),
            InventorySlot.Class        => new Equipment(ProfileColorItemId, AvatarFrameItemId, BackgroundItemId, PetItemId, itemId, WeaponItemId),
            InventorySlot.Weapon       => new Equipment(ProfileColorItemId, AvatarFrameItemId, BackgroundItemId, PetItemId, ClassItemId, itemId),
            _ => throw new InvalidOperationException($"Slot {slot} cannot be equipped")
        };
    }

    public Equipment UnequipSlot(InventorySlot slot)
    {
        return slot switch
        {
            InventorySlot.ProfileColor => new Equipment(null, AvatarFrameItemId, BackgroundItemId, PetItemId, ClassItemId, WeaponItemId),
            InventorySlot.AvatarFrame  => new Equipment(ProfileColorItemId, null, BackgroundItemId, PetItemId, ClassItemId, WeaponItemId),
            InventorySlot.Background   => new Equipment(ProfileColorItemId, AvatarFrameItemId, null, PetItemId, ClassItemId, WeaponItemId),
            InventorySlot.Pet          => new Equipment(ProfileColorItemId, AvatarFrameItemId, BackgroundItemId, null, ClassItemId, WeaponItemId),
            InventorySlot.Class        => new Equipment(ProfileColorItemId, AvatarFrameItemId, BackgroundItemId, PetItemId, null, WeaponItemId),
            InventorySlot.Weapon       => new Equipment(ProfileColorItemId, AvatarFrameItemId, BackgroundItemId, PetItemId, ClassItemId, null),
            _ => this
        };
    }
}
