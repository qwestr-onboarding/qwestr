using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.DeleteShopItem;

public class DeleteShopItemCommandHandler : IRequestHandler<DeleteShopItemCommand, ActionResult>
{
    private readonly IShopItemRepository _shopItemRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPlayerProfileRepository _playerProfileRepository;

    public DeleteShopItemCommandHandler(
        IShopItemRepository shopItemRepository,
        IInventoryRepository inventoryRepository,
        IPlayerProfileRepository playerProfileRepository)
    {
        _shopItemRepository = shopItemRepository;
        _inventoryRepository = inventoryRepository;
        _playerProfileRepository = playerProfileRepository;
    }

    public async Task<ActionResult> Handle(DeleteShopItemCommand request, CancellationToken ct)
    {
        var item = await _shopItemRepository.GetByIdAsync(request.ItemId, ct);
        if (item is null || item.TenantId != request.TenantId)
        {
            return ActionResult.Failure("ShopItem.NotFound");
        }

        if (item.Slot != InventorySlot.Consumable)
        {
            var profiles = await _playerProfileRepository.GetByTenantAsync(request.TenantId, ct);
            foreach (var profile in profiles)
            {
                var equippedId = GetEquippedId(profile.Equipment, item.Slot);
                if (equippedId == request.ItemId)
                {
                    profile.Unequip(item.Slot);
                }
            }
        }

        await _inventoryRepository.DeleteAllByShopItemAsync(request.ItemId, ct);
        _shopItemRepository.Delete(item);
        await _shopItemRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }

    private static Guid? GetEquippedId(Domain.ValueObjects.Equipment eq, InventorySlot slot) => slot switch
    {
        InventorySlot.ProfileColor => eq.ProfileColorItemId,
        InventorySlot.AvatarFrame  => eq.AvatarFrameItemId,
        InventorySlot.Background   => eq.BackgroundItemId,
        InventorySlot.Pet          => eq.PetItemId,
        InventorySlot.Class        => eq.ClassItemId,
        InventorySlot.Weapon       => eq.WeaponItemId,
        _                          => null
    };
}
