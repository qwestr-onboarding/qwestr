using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class ScenariosApi(QwestrApiClient api)
{
    public Task<ApiResult<PagedListResponse<ScenarioListItemResponse>>> GetListAsync(
        int page, int pageSize, string? tag = null, CancellationToken ct = default)
    {
        var url = $"/api/onboarding/scenarios?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(tag))
            url += $"&tag={Uri.EscapeDataString(tag)}";
        return api.GetAsync<PagedListResponse<ScenarioListItemResponse>>(url, ct);
    }

    public Task<ApiResult<ScenarioDetailResponse>> GetAsync(
        Guid id, CancellationToken ct = default)
        => api.GetAsync<ScenarioDetailResponse>($"/api/onboarding/scenarios/{id}", ct);

    public Task<ApiResult<IdResponse>> CreateAsync(
        CreateScenarioRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/onboarding/scenarios", request, ct);

    public Task<ApiResult> UpdateAsync(
        Guid id, UpdateScenarioRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/onboarding/scenarios/{id}", request, ct);

    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/onboarding/scenarios/{id}", ct);

    public Task<ApiResult> PublishAsync(Guid id, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/scenarios/{id}/publish", new { }, ct);

    public Task<ApiResult> UnpublishAsync(Guid id, CancellationToken ct = default)
        => api.PostAsync($"/api/onboarding/scenarios/{id}/unpublish", new { }, ct);

    public Task<ApiResult<IdResponse>> DuplicateAsync(Guid id, CancellationToken ct = default)
        => api.PostAsync<IdResponse>($"/api/onboarding/scenarios/{id}/duplicate", new { }, ct);

    public Task<ApiResult<AiGenerationStatusResponse>> GetAiStatusAsync(CancellationToken ct = default)
        => api.GetAsync<AiGenerationStatusResponse>("/api/onboarding/ai/status", ct);

    public Task<ApiResult<GenerateOnboardingScenarioResponse>> GenerateScenarioAsync(
        GenerateOnboardingScenarioRequest request, CancellationToken ct = default)
        => api.PostAsync<GenerateOnboardingScenarioResponse>("/api/onboarding/scenarios/generate", request, ct);
}