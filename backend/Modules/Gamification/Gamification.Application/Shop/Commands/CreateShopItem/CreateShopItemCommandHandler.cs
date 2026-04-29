using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.CreateShopItem;

public class CreateShopItemCommandHandler : IRequestHandler<CreateShopItemCommand, ActionResult<Guid>>
{
    private readonly IShopItemRepository _shopItemRepository;

    public CreateShopItemCommandHandler(IShopItemRepository shopItemRepository)
    {
        _shopItemRepository = shopItemRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateShopItemCommand request, CancellationToken ct)
    {
        var result = ShopItem.Create(
            request.TenantId,
            request.Slot,
            request.Name,
            request.Description,
            request.IconUrl,
            request.AssetData,
            request.Price,
            request.RequiredLevel,
            request.StackingPolicy);

        if (result.IsFailure)
        {
            return ActionResult<Guid>.Failure(result.Error);
        }

        await _shopItemRepository.AddAsync(result.Value!, ct);
        await _shopItemRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(result.Value!.Id);
    }
}
