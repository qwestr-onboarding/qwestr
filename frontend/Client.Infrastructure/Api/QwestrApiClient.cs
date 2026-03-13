using System.Net.Http.Json;
using Client.Infrastructure.Api.Models;

namespace Client.Infrastructure.Api;

public class QwestrApiClient
{
    private readonly HttpClient _http;

    public QwestrApiClient(HttpClient http) => _http = http;

    public async Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(url, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult<T>> PostAsync<T>(string url, object body, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(url, body, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult> PostAsync(string url, object body, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(url, body, ct);
        return await ParseResponse(response, ct);
    }

    public async Task<ApiResult<T>> PutAsync<T>(string url, object body, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync(url, body, ct);
        return await ParseResponse<T>(response, ct);
    }

    public async Task<ApiResult> PutAsync(string url, object body, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync(url, body, ct);
        return await ParseResponse(response, ct);
    }

    public async Task<ApiResult> DeleteAsync(string url, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(url, ct);
        return await ParseResponse(response, ct);
    }

    public async Task<ApiResult> DeleteAsync(string url, object body, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = JsonContent.Create(body)
        };
        var response = await _http.SendAsync(request, ct);
        return await ParseResponse(response, ct);
    }

    private static async Task<ApiResult<T>> ParseResponse<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(ct);
            return ApiResult<T>.Success(data!);
        }
        var error = await response.Content.ReadAsStringAsync(ct);
        return ApiResult<T>.Failure(response.StatusCode, error);
    }

    private static async Task<ApiResult> ParseResponse(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return ApiResult.Success();
        var error = await response.Content.ReadAsStringAsync(ct);
        return ApiResult.Failure(response.StatusCode, error);
    }
}