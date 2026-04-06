using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

namespace Client.Infrastructure.Api.Endpoints;

public class OptionsApi(QwestrApiClient api)
{
    public Task<ApiResult<IdResponse>> CreateAsync(
        CreateQuestionOptionRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/onboarding/options", request, ct);

    public Task<ApiResult> UpdateAsync(
        Guid id, UpdateQuestionOptionRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/options/{id}", request, ct);

    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/onboarding/options/{id}", ct);

    public Task<ApiResult> MoveAsync(
        Guid id, MoveRequest request, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/options/{id}/move", request, ct);
}