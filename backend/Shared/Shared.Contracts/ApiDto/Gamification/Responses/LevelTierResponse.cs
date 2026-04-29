namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record LevelTierResponse(
    Guid Id,
    int Order,
    string Title,
    int FromLevel,
    int ToLevel,
    string Color,
    string? FrameUrl);

public record LevelTierListResponse(List<LevelTierResponse> Items);
