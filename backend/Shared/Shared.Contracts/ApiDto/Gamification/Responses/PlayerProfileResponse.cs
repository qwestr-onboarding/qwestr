namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record PlayerProfileResponse(
    Guid Id,
    Guid UserId,
    Guid TenantId,
    int TotalXp,
    int AvailableCoins,
    int CurrentLevel,
    int XpToNextLevel,
    string? TierTitle,
    string? TierColor,
    string? TierFrameUrl,
    string? Status,
    EquipmentResponse Equipment,
    DateTime LastActivityAt,
    DateTime CreatedAt);

public record EquipmentResponse(
    EquippedItemResponse? ProfileColor,
    EquippedItemResponse? AvatarFrame,
    EquippedItemResponse? Background,
    EquippedItemResponse? Pet,
    EquippedItemResponse? Class,
    EquippedItemResponse? Weapon);

public record EquippedItemResponse(
    Guid Id,
    string Name,
    string? IconUrl,
    string AssetData);
