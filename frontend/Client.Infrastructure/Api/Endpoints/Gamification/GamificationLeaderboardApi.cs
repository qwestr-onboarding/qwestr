using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

namespace Client.Infrastructure.Api.Endpoints.Gamification;

public class GamificationLeaderboardApi(QwestrApiClient api)
{
    public Task<ApiResult<LeaderboardListResponse>> GetLeaderboardAsync(
        string? period = null, int topN = 50, CancellationToken ct = default)
    {
        var url = $"/api/gamification/leaderboard?topN={topN}";
        if (!string.IsNullOrEmpty(period)) url += $"&period={period}";
        return api.GetAsync<LeaderboardListResponse>(url, ct);
    }

    public Task<ApiResult<MyLeaderboardPositionResponse>> GetMyLeaderboardPositionAsync(
        string? period = null, CancellationToken ct = default)
    {
        var url = "/api/gamification/leaderboard/me";
        if (!string.IsNullOrEmpty(period)) url += $"?period={period}";
        return api.GetAsync<MyLeaderboardPositionResponse>(url, ct);
    }
}
