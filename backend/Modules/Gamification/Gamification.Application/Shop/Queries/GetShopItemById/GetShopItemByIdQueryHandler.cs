using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItemById;

public class GetShopItemByIdQueryHandler : IRequestHandler<GetShopItemByIdQuery, ActionResult<ShopItemResponse>>
{
    private readonly IShopItemRepository _shopItemRepository;
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public GetShopItemByIdQueryHandler(
        IShopItemRepository shopItemRepository,
        IPlayerProfileRepository profileRepository,
        IInventoryRepository inventoryRepository)
    {
        _shopItemRepository = shopItemRepository;
        _profileRepository = profileRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<ActionResult<ShopItemResponse>> Handle(GetShopItemByIdQuery request, CancellationToken ct)
    {
        var item = await _shopItemRepository.GetByIdAsync(request.ItemId, ct);
        if (item == null || item.TenantId != request.TenantId)
        {
            return ActionResult<ShopItemResponse>.Failure("ShopItem.NotFound");
        }

        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);

        var isOwned = false;
        var isAvailable = false;
        if (profile != null)
        {
            var inventoryItem = await _inventoryRepository.GetByProfileAndShopItemAsync(profile.Id, item.Id, ct);
            isOwned = inventoryItem != null;
            isAvailable = !isOwned && item.CanBePurchasedBy(profile.CurrentLevel, profile.AvailableCoins).IsSuccess;
        }

        var response = new ShopItemResponse(
            item.Id,
            item.Slot.ToString(),
            item.Name,
            item.Description,
            item.IconUrl,
            item.Price,
            item.RequiredLevel,
            item.StackingPolicy.ToString(),
            item.IsActive,
            isOwned,
            isAvailable);

        return ActionResult<ShopItemResponse>.Success(response);
    }
}
