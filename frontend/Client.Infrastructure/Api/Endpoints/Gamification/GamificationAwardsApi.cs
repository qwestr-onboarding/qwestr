using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

namespace Client.Infrastructure.Api.Endpoints.Gamification;

public class GamificationAwardsApi(QwestrApiClient api)
{
    public Task<ApiResult<AwardListResponse>> GetAwardsAsync(CancellationToken ct = default)
        => api.GetAsync<AwardListResponse>("/api/gamification/awards", ct);

    public Task<ApiResult<AwardListResponse>> GetAwardsForManagementAsync(CancellationToken ct = default)
        => api.GetAsync<AwardListResponse>("/api/gamification/awards/manage", ct);

    public Task<ApiResult<IdResponse>> CreateAutomaticAwardAsync(
        CreateAutomaticAwardRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/gamification/awards/automatic", request, ct);

    public Task<ApiResult<IdResponse>> CreateManualAwardAsync(
        CreateManualAwardRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/gamification/awards/manual", request, ct);

    public Task<ApiResult> UpdateAwardAsync(
        Guid id, UpdateAwardRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/gamification/awards/{id}", request, ct);

    public Task<ApiResult> DeactivateAwardAsync(Guid id, CancellationToken ct = default)
        => api.PostAsync($"/api/gamification/awards/{id}/deactivate", new { }, ct);

    public Task<ApiResult> IssueManualAwardAsync(
        Guid id, IssueManualAwardRequest request, CancellationToken ct = default)
        => api.PostAsync($"/api/gamification/awards/{id}/issue", request, ct);
}
