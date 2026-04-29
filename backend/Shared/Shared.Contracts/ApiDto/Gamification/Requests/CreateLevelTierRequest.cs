namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record CreateLevelTierRequest(
    int Order,
    string Title,
    int FromLevel,
    int ToLevel,
    string Color,
    string? FrameUrl);
