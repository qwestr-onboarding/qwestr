namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record UpdateShopItemRequest(
    string Name,
    string? Description,
    string IconUrl,
    string AssetData,
    int Price,
    int? RequiredLevel);
