using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.DeactivateShopItem;

public class DeactivateShopItemCommandHandler : IRequestHandler<DeactivateShopItemCommand, ActionResult>
{
    private readonly IShopItemRepository _shopItemRepository;

    public DeactivateShopItemCommandHandler(IShopItemRepository shopItemRepository)
    {
        _shopItemRepository = shopItemRepository;
    }

    public async Task<ActionResult> Handle(DeactivateShopItemCommand request, CancellationToken ct)
    {
        var item = await _shopItemRepository.GetByIdAsync(request.Id, ct);
        if (item == null || item.TenantId != request.TenantId)
        {
            return ActionResult.Failure("ShopItem.NotFound");
        }

        item.Deactivate();
        await _shopItemRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
