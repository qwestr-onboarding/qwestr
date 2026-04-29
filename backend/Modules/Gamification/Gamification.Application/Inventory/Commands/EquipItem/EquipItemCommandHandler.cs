using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Events;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Inventory.Commands.EquipItem;

public class EquipItemCommandHandler : IRequestHandler<EquipItemCommand, ActionResult>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IShopItemRepository _shopItemRepository;
    private readonly IPublisher _publisher;

    public EquipItemCommandHandler(
        IPlayerProfileRepository profileRepository,
        IInventoryRepository inventoryRepository,
        IShopItemRepository shopItemRepository,
        IPublisher publisher)
    {
        _profileRepository = profileRepository;
        _inventoryRepository = inventoryRepository;
        _shopItemRepository = shopItemRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(EquipItemCommand request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult.Failure("Profile.NotFound");
        }

        var inventoryItem = await _inventoryRepository.GetByProfileAndShopItemAsync(profile.Id, request.ShopItemId, ct);
        if (inventoryItem == null)
        {
            return ActionResult.Failure("Inventory.ItemNotOwned");
        }

        var shopItem = await _shopItemRepository.GetByIdAsync(request.ShopItemId, ct);
        if (shopItem == null)
        {
            return ActionResult.Failure("ShopItem.NotFound");
        }

        var equipResult = profile.Equip(shopItem.Slot, shopItem.Id);
        if (equipResult.IsFailure)
        {
            return ActionResult.Failure(equipResult.Error);
        }

        await _profileRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new ItemEquippedEvent(request.TenantId, profile.Id, profile.UserId, shopItem.Slot, shopItem.Id), ct);

        return ActionResult.Success();
    }
}
