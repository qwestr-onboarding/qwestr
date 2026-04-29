using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Inventory.Queries.GetMyInventory;

public class GetMyInventoryQueryHandler : IRequestHandler<GetMyInventoryQuery, ActionResult<InventoryListResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IShopItemRepository _shopItemRepository;

    public GetMyInventoryQueryHandler(
        IPlayerProfileRepository profileRepository,
        IInventoryRepository inventoryRepository,
        IShopItemRepository shopItemRepository)
    {
        _profileRepository = profileRepository;
        _inventoryRepository = inventoryRepository;
        _shopItemRepository = shopItemRepository;
    }

    public async Task<ActionResult<InventoryListResponse>> Handle(GetMyInventoryQuery request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult<InventoryListResponse>.Failure("Profile.NotFound");
        }

        var inventory = await _inventoryRepository.GetByProfileAsync(profile.Id, ct);
        var shopItems = await _shopItemRepository.GetByTenantAsync(request.TenantId, activeOnly: false, ct);
        var shopItemDict = shopItems.ToDictionary(s => s.Id);

        var result = inventory
            .Where(i => shopItemDict.ContainsKey(i.ShopItemId))
            .Select(i =>
            {
                var shopItem = shopItemDict[i.ShopItemId];
                return new InventoryItemResponse(
                    i.Id,
                    i.ShopItemId,
                    shopItem.Slot.ToString(),
                    shopItem.Name,
                    shopItem.IconUrl,
                    i.Quantity,
                    i.AcquiredAt);
            }).ToList();

        return ActionResult<InventoryListResponse>.Success(new InventoryListResponse(result));
    }
}
