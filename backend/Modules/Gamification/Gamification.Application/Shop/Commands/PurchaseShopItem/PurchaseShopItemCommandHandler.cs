using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.Events;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Application.Shop.Commands.PurchaseShopItem;

public class PurchaseShopItemCommandHandler : IRequestHandler<PurchaseShopItemCommand, ActionResult>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IShopItemRepository _shopItemRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPointsTransactionRepository _transactionRepository;
    private readonly IPublisher _publisher;

    public PurchaseShopItemCommandHandler(
        IPlayerProfileRepository profileRepository,
        IShopItemRepository shopItemRepository,
        IInventoryRepository inventoryRepository,
        IPointsTransactionRepository transactionRepository,
        IPublisher publisher)
    {
        _profileRepository = profileRepository;
        _shopItemRepository = shopItemRepository;
        _inventoryRepository = inventoryRepository;
        _transactionRepository = transactionRepository;
        _publisher = publisher;
    }

    public async Task<ActionResult> Handle(PurchaseShopItemCommand request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult.Failure("Profile.NotFound");
        }

        var shopItem = await _shopItemRepository.GetByIdAsync(request.ShopItemId, ct);
        if (shopItem == null || shopItem.TenantId != request.TenantId)
        {
            return ActionResult.Failure("ShopItem.NotFound");
        }

        var canPurchase = shopItem.CanBePurchasedBy(profile.CurrentLevel, profile.AvailableCoins);
        if (canPurchase.IsFailure)
        {
            return ActionResult.Failure(canPurchase.Error);
        }

        var existingItem = await _inventoryRepository.GetByProfileAndShopItemAsync(profile.Id, shopItem.Id, ct);
        if (existingItem != null)
        {
            if (shopItem.StackingPolicy == StackingPolicy.Unique)
            {
                return ActionResult.Failure("ShopItem.AlreadyOwned");
            }
            existingItem.IncreaseQuantity(1);
        }
        else
        {
            var newItem = InventoryItem.Create(request.TenantId, profile.Id, shopItem.Id, 1, InventoryAcquisitionSource.Purchase);
            await _inventoryRepository.AddAsync(newItem, ct);
        }

        var spendResult = profile.SpendCoins(shopItem.Price);
        if (spendResult.IsFailure)
        {
            return ActionResult.Failure(spendResult.Error);
        }

        var coinTx = CoinTransaction.CreateSpent(request.TenantId, profile.Id, shopItem.Price, shopItem.Id, $"Purchased: {shopItem.Name}");
        await _transactionRepository.AddCoinTransactionAsync(coinTx, ct);

        await _profileRepository.SaveChangesAsync(ct);
        await _inventoryRepository.SaveChangesAsync(ct);

        await _publisher.Publish(new ItemPurchasedEvent(request.TenantId, profile.Id, profile.UserId, shopItem.Id, shopItem.Price), ct);

        return ActionResult.Success();
    }
}
