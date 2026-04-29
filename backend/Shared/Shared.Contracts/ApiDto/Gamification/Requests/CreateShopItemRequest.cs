namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record CreateShopItemRequest(
    string Slot,
    string Name,
    string? Description,
    string IconUrl,
    string AssetData,
    int Price,
    int? RequiredLevel,
    string StackingPolicy);
