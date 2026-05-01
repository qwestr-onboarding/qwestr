using System.Net;
using System.Net.Http.Headers;
using Client.Infrastructure.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

namespace Client.Infrastructure.Api.Endpoints.Gamification;

public class GamificationShopApi(QwestrApiClient api)
{
    public Task<ApiResult<ShopItemsList>> GetShopItemsAsync(CancellationToken ct = default)
        => api.GetAsync<ShopItemsList>("/api/gamification/shop", ct);

    public Task<ApiResult<ShopItemResponse>> GetShopItemByIdAsync(Guid id, CancellationToken ct = default)
        => api.GetAsync<ShopItemResponse>($"/api/gamification/shop/{id}", ct);

    public Task<ApiResult<ShopItemManagementListResponse>> GetShopItemsForManagementAsync(CancellationToken ct = default)
        => api.GetAsync<ShopItemManagementListResponse>("/api/gamification/shop/manage", ct);

    public Task<ApiResult<IdResponse>> CreateShopItemAsync(
        CreateShopItemRequest request, CancellationToken ct = default)
        => api.PostAsync<IdResponse>("/api/gamification/shop", request, ct);

    public Task<ApiResult> UpdateShopItemAsync(
        Guid id, UpdateShopItemRequest request, CancellationToken ct = default)
        => api.PutAsync($"/api/gamification/shop/{id}", request, ct);

    public Task<ApiResult> DeactivateShopItemAsync(Guid id, CancellationToken ct = default)
        => api.PostAsync($"/api/gamification/shop/{id}/deactivate", new { }, ct);

    public Task<ApiResult> PurchaseShopItemAsync(Guid id, CancellationToken ct = default)
        => api.PostAsync($"/api/gamification/shop/{id}/purchase", new { }, ct);

    public async Task<ApiResult<string>> UploadShopAssetAsync(IBrowserFile file, CancellationToken ct = default)
    {
        using var content = new MultipartFormDataContent();
        var stream = file.OpenReadStream(maxAllowedSize: 10_000_000, ct);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);
        var result = await api.PostContentAsync<UploadUrlResponse>("/api/gamification/shop/assets/upload", content, ct);
        if (!result.IsSuccess) { return ApiResult<string>.Failure(result.StatusCode, result.Error ?? "Upload failed"); }
        return ApiResult<string>.Success(result.Data!.Url);
    }

    public Task<ApiResult> DeleteShopItemAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/gamification/shop/{id}", ct);

    public string? ResolveAssetUrl(string? path) => api.ResolveAssetUrl(path);
}
