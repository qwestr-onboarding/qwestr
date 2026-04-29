using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.UpdateShopItem;

public class UpdateShopItemCommandHandler : IRequestHandler<UpdateShopItemCommand, ActionResult>
{
    private readonly IShopItemRepository _shopItemRepository;

    public UpdateShopItemCommandHandler(IShopItemRepository shopItemRepository)
    {
        _shopItemRepository = shopItemRepository;
    }

    public async Task<ActionResult> Handle(UpdateShopItemCommand request, CancellationToken ct)
    {
        var item = await _shopItemRepository.GetByIdAsync(request.Id, ct);
        if (item == null || item.TenantId != request.TenantId)
        {
            return ActionResult.Failure("ShopItem.NotFound");
        }

        item.Update(request.Name, request.Description, request.IconUrl, request.AssetData, request.Price, request.RequiredLevel);
        await _shopItemRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
