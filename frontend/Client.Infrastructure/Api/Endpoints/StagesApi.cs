using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

namespace Client.Infrastructure.Api.Endpoints;

public class StagesApi(QwestrApiClient api)
{
    public Task<ApiResult<IdResponse>> CreateAsync(
        CreateStageRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/onboarding/stages", request, ct);

    public Task<ApiResult> UpdateAsync(
        Guid id, UpdateStageRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/stages/{id}", request, ct);

    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/onboarding/stages/{id}", ct);

    public Task<ApiResult> MoveAsync(
        Guid id, MoveRequest request, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/stages/{id}/move", request, ct);
}