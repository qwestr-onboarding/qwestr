using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Client.Infrastructure.Api.Models;
using Client.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;

namespace Client.Infrastructure.Api;

public class QwestrApiClient
{
    private readonly HttpClient _http;
    private readonly TokenStorageService _tokenStorage;
    private readonly string _publicBaseUrl;

    public QwestrApiClient(HttpClient http, TokenStorageService tokenStorage, IConfiguration config)
    {
        _http = http;
        _tokenStorage = tokenStorage;
        _publicBaseUrl = (config["QwestrApi:PublicBaseUrl"] ?? http.BaseAddress?.ToString() ?? "").TrimEnd('/');
    }

    /// <summary>
    /// Превращает относительный путь к ассету backend'а в абсолютный URL для браузера.
    /// Если путь уже абсолютный — возвращает как есть.
    /// </summary>
    public string? ResolveAssetUrl(string? path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (path.StartsWith("http://") || path.StartsWith("https://")) return path;
        if (path.StartsWith("/")) return $"{_publicBaseUrl}{path}";
        return null;
    }

    public async Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult<T>> PostAsync<T>(string url, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body)
        };
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult> PostAsync(string url, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body)
        };
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse(response, ct);
    }

    public async Task<ApiResult<T>> PutAsync<T>(string url, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(body)
        };
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult> PutAsync(string url, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(body)
        };
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse(response, ct);
    }

    public async Task<ApiResult> DeleteAsync(string url, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse(response, ct);
    }

    public async Task<ApiResult<T>> PostContentAsync<T>(string url, HttpContent content, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult<byte[]>> GetBytesAsync(string url, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        if (response.IsSuccessStatusCode)
        {
            var bytes = await response.Content.ReadAsByteArrayAsync(ct);
            return ApiResult<byte[]>.Success(bytes);
        }
        var error = await ReadErrorAsync(response, ct);
        return ApiResult<byte[]>.Failure(response.StatusCode, error);
    }

    public async Task<ApiResult> DeleteAsync(string url, object body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = JsonContent.Create(body)
        };
        ApplyAuth(request);
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse(response, ct);
    }

    private void ApplyAuth(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenStorage.AccessToken);
    }

    private static async Task<ApiResult<T>> ParseResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(ct);
            return ApiResult<T>.Success(data!);
        }
        var error = await ReadErrorAsync(response, ct);
        return ApiResult<T>.Failure(response.StatusCode, error);
    }

    private static async Task<ApiResult> ParseResponse(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return ApiResult.Success();
        var error = await ReadErrorAsync(response, ct);
        return ApiResult.Failure(response.StatusCode, error);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        try
        {
            var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String)
                return title.GetString()!;
            if (doc.RootElement.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.String)
                return error.GetString()!;
        }
        catch (JsonException) { }
        return body;
    }
}