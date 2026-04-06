using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class AssignmentsApi(QwestrApiClient api)
{
    public Task<ApiResult<PagedListResponse<AssignmentListItemResponse>>> GetListAsync(
        int page, int pageSize, Guid? scenarioId = null, Guid? userId = null, CancellationToken ct = default)
    {
        var url = $"/api/onboarding/assignments?page={page}&pageSize={pageSize}";
        if (scenarioId.HasValue)
            url += $"&scenarioId={scenarioId.Value}";
        if (userId.HasValue)
            url += $"&userId={userId.Value}";
        return api.GetAsync<PagedListResponse<AssignmentListItemResponse>>(url, ct);
    }

    public Task<ApiResult<AssignmentDetailResponse>> GetAsync(
        Guid id, CancellationToken ct = default)
        => api.GetAsync<AssignmentDetailResponse>($"/api/onboarding/assignments/{id}", ct);

    public Task<ApiResult<IdResponse>> AssignAsync(
        AssignScenarioRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/onboarding/assignments", request, ct);

    public Task<ApiResult> UnassignAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/onboarding/assignments/{id}", ct);
}