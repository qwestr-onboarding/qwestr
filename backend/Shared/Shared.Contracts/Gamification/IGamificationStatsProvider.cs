namespace Qwestr.Shared.Contracts.Gamification;

public interface IGamificationStatsProvider
{
    Task<GamificationTenantStats> GetTenantStatsAsync(Guid tenantId, CancellationToken ct);
}

public sealed record GamificationTenantStats(
    int ActivePlayers,
    double AverageLevel,
    IReadOnlyList<TopAwardStat> TopAwards);

public sealed record TopAwardStat(Guid AwardId, string Code, int EarnedCount);
