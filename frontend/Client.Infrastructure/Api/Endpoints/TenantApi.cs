using System.Net.Http.Headers;
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

    public async Task<ApiResult<UploadLogoResponse>> UploadLogoAsync(
        Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        return await api.PostContentAsync<UploadLogoResponse>("/api/identity/tenant/logo", content, ct);
    }

    public Task<ApiResult> DeleteLogoAsync(CancellationToken ct = default)
        => api.DeleteAsync("/api/identity/tenant/logo", ct);

    public string? ResolveAssetUrl(string? path) => api.ResolveAssetUrl(path);
}