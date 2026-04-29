using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItems;

public class GetShopItemsQueryHandler : IRequestHandler<GetShopItemsQuery, ActionResult<ShopItemsList>>
{
    private readonly IShopItemRepository _shopItemRepository;
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public GetShopItemsQueryHandler(
        IShopItemRepository shopItemRepository,
        IPlayerProfileRepository profileRepository,
        IInventoryRepository inventoryRepository)
    {
        _shopItemRepository = shopItemRepository;
        _profileRepository = profileRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<ActionResult<ShopItemsList>> Handle(GetShopItemsQuery request, CancellationToken ct)
    {
        var items = await _shopItemRepository.GetByTenantAsync(request.TenantId, activeOnly: true, ct);
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);

        HashSet<Guid> ownedItemIds = new();
        if (profile != null)
        {
            var inventory = await _inventoryRepository.GetByProfileAsync(profile.Id, ct);
            ownedItemIds = inventory.Select(i => i.ShopItemId).ToHashSet();
        }

        var result = items.Select(item =>
        {
            var isOwned = ownedItemIds.Contains(item.Id);
            var isAvailable = !isOwned && profile != null && item.CanBePurchasedBy(profile.CurrentLevel, profile.AvailableCoins).IsSuccess;

            return new ShopItemResponse(
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
        }).ToList();
        var resultDto = new ShopItemsList
        {
            Items = result
        };
        return ActionResult<ShopItemsList>.Success(resultDto);
    }
}
