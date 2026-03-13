using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class TenantApi(QwestrApiClient api)
{
    public Task<ApiResult<TenantInfoResponse>> GetTenantAsync(CancellationToken ct = default)
        => api.GetAsync<TenantInfoResponse>("/api/identity/tenant", ct);

    public Task<ApiResult<TenantInfoResponse>> LookupBySubdomainAsync(
        string subdomain, CancellationToken ct = default)
        => api.GetAsync<TenantInfoResponse>($"/api/identity/tenant/lookup/{subdomain}", ct);

    public Task<ApiResult> UpdateTenantAsync(
        UpdateTenantRequest request, CancellationToken ct = default)
        => api.PutAsync("/api/identity/tenant", request, ct);

    public Task<ApiResult<TenantSettingsResponse>> GetSettingsAsync(CancellationToken ct = default)
        => api.GetAsync<TenantSettingsResponse>("/api/identity/tenant/settings", ct);

    public Task<ApiResult> UpdateSettingsAsync(
        UpdateTenantSettingsRequest request, CancellationToken ct = default)
        => api.PutAsync("/api/identity/tenant/settings", request, ct);
}