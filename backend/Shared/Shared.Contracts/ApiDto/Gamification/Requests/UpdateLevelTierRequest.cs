namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record UpdateLevelTierRequest(
    string Title,
    int FromLevel,
    int ToLevel,
    string Color,
    string? FrameUrl);
