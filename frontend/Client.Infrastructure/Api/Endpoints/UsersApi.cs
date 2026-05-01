using System.Net.Http.Headers;
using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class UsersApi(QwestrApiClient api)
{
    public Task<ApiResult<UserProfileResponse>> GetCurrentUserAsync(CancellationToken ct = default)
        => api.GetAsync<UserProfileResponse>("/api/identity/users/me", ct);

    public Task<ApiResult> UpdateProfileAsync(
        UpdateProfileRequest request, CancellationToken ct = default)
        => api.PutAsync("/api/identity/users/me", request, ct);

    public Task<ApiResult> ChangePasswordAsync(
        ChangePasswordRequest request, CancellationToken ct = default)
        => api.PutAsync("/api/identity/users/me/password", request, ct);

    public Task<ApiResult<TwoFactorSetupResponse>> SetupTwoFactorAsync(CancellationToken ct = default)
        => api.PostAsync<TwoFactorSetupResponse>("/api/identity/users/me/2fa/setup", new { }, ct);

    public Task<ApiResult<BackupCodesResponse>> ConfirmTwoFactorAsync(
        ConfirmTwoFactorRequest request, CancellationToken ct = default)
        => api.PostAsync<BackupCodesResponse>("/api/identity/users/me/2fa/confirm", request, ct);

    public Task<ApiResult> DisableTwoFactorAsync(
        DisableTwoFactorRequest request, CancellationToken ct = default)
        => api.DeleteAsync("/api/identity/users/me/2fa", request, ct);

    public Task<ApiResult<PagedListResponse<UserListItemResponse>>> GetUsersListAsync(
        int page, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var url = $"/api/identity/users?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(search))
            url += $"&search={Uri.EscapeDataString(search)}";
        return api.GetAsync<PagedListResponse<UserListItemResponse>>(url, ct);
    }

    public Task<ApiResult<UserProfileResponse>> GetUserByIdAsync(
        Guid userId, CancellationToken ct = default)
        => api.GetAsync<UserProfileResponse>($"/api/identity/users/{userId}", ct);

    public Task<ApiResult> DeactivateUserAsync(Guid userId, CancellationToken ct = default)
        => api.PutAsync($"/api/identity/users/{userId}/deactivate", new { }, ct);

    public Task<ApiResult> ActivateUserAsync(Guid userId, CancellationToken ct = default)
        => api.PutAsync($"/api/identity/users/{userId}/activate", new { }, ct);

    public Task<ApiResult> ChangeUserRoleAsync(
        Guid userId, ChangeUserRoleRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/identity/users/{userId}/role", request, ct);

    public async Task<ApiResult<UploadAvatarResponse>> UploadAvatarAsync(
        Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        return await api.PostContentAsync<UploadAvatarResponse>("/api/identity/users/me/avatar", content, ct);
    }

    public Task<ApiResult> DeleteAvatarAsync(CancellationToken ct = default)
        => api.DeleteAsync("/api/identity/users/me/avatar", ct);

    public string? ResolveAssetUrl(string? path) => api.ResolveAssetUrl(path);
}