namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record LeaderboardEntryResponse(
    int Position,
    Guid UserId,
    string DisplayName,
    int Level,
    string? TierTitle,
    string? TierColor,
    int TotalXp,
    int PeriodXp);

public record MyLeaderboardPositionResponse(
    int Position,
    int TotalXp,
    int PeriodXp,
    int Level,
    string? TierTitle);

public record LeaderboardListResponse(List<LeaderboardEntryResponse> Items);
