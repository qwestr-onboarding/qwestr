namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record InventoryItemResponse(
    Guid Id,
    Guid ShopItemId,
    string Slot,
    string Name,
    string IconUrl,
    int Quantity,
    DateTime AcquiredAt);

public record InventoryListResponse(List<InventoryItemResponse> Items);
