using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

namespace Client.Infrastructure.Api.Endpoints;

public class QuestionsApi(QwestrApiClient api)
{
    public Task<ApiResult<IdResponse>> CreateAsync(
        CreateQuestionRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/onboarding/questions", request, ct);

    public Task<ApiResult> UpdateAsync(
        Guid id, UpdateQuestionRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/questions/{id}", request, ct);

    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/onboarding/questions/{id}", ct);

    public Task<ApiResult> MoveAsync(
        Guid id, MoveRequest request, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/questions/{id}/move", request, ct);
}