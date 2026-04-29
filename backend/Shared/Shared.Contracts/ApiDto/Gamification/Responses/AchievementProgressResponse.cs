namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record AchievementProgressResponse(
    Guid AwardId,
    string Code,
    string Title,
    string IconUrl,
    int CurrentValue,
    int TargetValue,
    bool IsCompleted,
    DateTime? CompletedAt);

public record AchievementProgressListResponse(List<AchievementProgressResponse> Items);
