namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record ShopItemResponse(
    Guid Id,
    string Slot,
    string Name,
    string? Description,
    string IconUrl,
    int Price,
    int? RequiredLevel,
    string StackingPolicy,
    bool IsActive,
    bool IsOwned,
    bool IsAvailable);

public record ShopItemManagementResponse(
    Guid Id,
    string Slot,
    string Name,
    string? Description,
    string IconUrl,
    int Price,
    int? RequiredLevel,
    string StackingPolicy,
    bool IsActive);

public record ShopItemManagementListResponse(List<ShopItemManagementResponse> Items);
