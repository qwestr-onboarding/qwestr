using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

namespace Client.Infrastructure.Api.Endpoints.Gamification;

public class GamificationLevelTiersApi(QwestrApiClient api)
{
    public Task<ApiResult<LevelTierListResponse>> GetLevelTiersAsync(CancellationToken ct = default)
        => api.GetAsync<LevelTierListResponse>("/api/gamification/level-tiers", ct);

    public Task<ApiResult<IdResponse>> CreateLevelTierAsync(
        CreateLevelTierRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/gamification/level-tiers", request, ct);

    public Task<ApiResult> UpdateLevelTierAsync(
        Guid id, UpdateLevelTierRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/gamification/level-tiers/{id}", request, ct);

    public Task<ApiResult> DeleteLevelTierAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/gamification/level-tiers/{id}", ct);
}
