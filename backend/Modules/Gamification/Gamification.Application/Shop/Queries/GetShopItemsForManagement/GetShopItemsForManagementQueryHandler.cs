using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Queries.GetShopItemsForManagement;

public class GetShopItemsForManagementQueryHandler : IRequestHandler<GetShopItemsForManagementQuery, ActionResult<ShopItemManagementListResponse>>
{
    private readonly IShopItemRepository _shopItemRepository;

    public GetShopItemsForManagementQueryHandler(IShopItemRepository shopItemRepository)
    {
        _shopItemRepository = shopItemRepository;
    }

    public async Task<ActionResult<ShopItemManagementListResponse>> Handle(GetShopItemsForManagementQuery request, CancellationToken ct)
    {
        var items = await _shopItemRepository.GetByTenantAsync(request.TenantId, activeOnly: false, ct);

        var result = items.Select(item => new ShopItemManagementResponse(
            item.Id,
            item.Slot.ToString(),
            item.Name,
            item.Description,
            item.IconUrl,
            item.Price,
            item.RequiredLevel,
            item.StackingPolicy.ToString(),
            item.IsActive)).ToList();

        return ActionResult<ShopItemManagementListResponse>.Success(new ShopItemManagementListResponse(result));
    }
}
