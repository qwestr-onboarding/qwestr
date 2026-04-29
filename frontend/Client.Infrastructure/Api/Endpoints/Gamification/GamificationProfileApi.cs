using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

namespace Client.Infrastructure.Api.Endpoints.Gamification;

public class GamificationProfileApi(QwestrApiClient api)
{
    public Task<ApiResult<PlayerProfileResponse>> GetMyProfileAsync(CancellationToken ct = default)
        => api.GetAsync<PlayerProfileResponse>("/api/gamification/profiles/me", ct);

    public Task<ApiResult<PlayerProfileResponse>> GetProfileByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => api.GetAsync<PlayerProfileResponse>($"/api/gamification/profiles/{userId}", ct);

    public Task<ApiResult<PointsTransactionListResponse>> GetMyTransactionsAsync(
        DateTime? from = null, DateTime? to = null, int limit = 50, CancellationToken ct = default)
    {
        var url = $"/api/gamification/profiles/me/transactions?limit={limit}";
        if (from.HasValue) url += $"&from={from.Value:o}";
        if (to.HasValue) url += $"&to={to.Value:o}";
        return api.GetAsync<PointsTransactionListResponse>(url, ct);
    }

    public Task<ApiResult<EarnedAwardListResponse>> GetMyEarnedAwardsAsync(CancellationToken ct = default)
        => api.GetAsync<EarnedAwardListResponse>("/api/gamification/profiles/me/earned-awards", ct);

    public Task<ApiResult<AchievementProgressListResponse>> GetMyAchievementProgressAsync(CancellationToken ct = default)
        => api.GetAsync<AchievementProgressListResponse>("/api/gamification/profiles/me/progress", ct);
}
