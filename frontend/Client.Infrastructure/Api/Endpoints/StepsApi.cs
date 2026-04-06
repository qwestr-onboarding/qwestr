using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

namespace Client.Infrastructure.Api.Endpoints;

public class StepsApi(QwestrApiClient api)
{
    public Task<ApiResult<IdResponse>> CreateAsync(
        CreateStepRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/onboarding/steps", request, ct);

    public Task<ApiResult> UpdateAsync(
        Guid id, UpdateStepRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/steps/{id}", request, ct);

    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/onboarding/steps/{id}", ct);

    public Task<ApiResult> MoveAsync(
        Guid id, MoveRequest request, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/steps/{id}/move", request, ct);
}