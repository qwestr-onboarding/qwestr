using System.Text.Json;
using Microsoft.JSInterop;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;

namespace Client.Infrastructure.Auth;

public class BffAuthClient(IJSRuntime Js, TokenStorageService TokenStorage)
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<BffResponse<LoginResponse>> LoginAsync(
        LoginRequest request, CancellationToken ct = default)
    {
        var js = await Invoke("bffAuth.post", ct, "/bff/auth/login", request);
        return Parse<LoginResponse>(js);
    }

    public async Task<BffResponse<VerifyTwoFactorResponse>> VerifyTwoFactorAsync(
        VerifyTwoFactorRequest request, CancellationToken ct = default)
    {
        var js = await Invoke("bffAuth.post", ct, "/bff/auth/login/2fa", request);
        return Parse<VerifyTwoFactorResponse>(js);
    }

    public async Task<BffResponse<RegisterResponse>> RegisterAsync(
        RegisterTenantRequest request, CancellationToken ct = default)
    {
        var js = await Invoke("bffAuth.post", ct, "/bff/auth/register", request);
        return Parse<RegisterResponse>(js);
    }

    public async Task<BffResponse> LogoutAsync(CancellationToken ct = default)
    {
        var js = await Js.InvokeAsync<JsonElement>("bffAuth.post", ct, "/bff/auth/logout", null, TokenStorage.AccessToken);
        await ClearStoredTokenAsync();
        return ParseNoData(js);
    }

    public async Task<BffResponse> ForgotPasswordAsync(
        RequestPasswordResetRequest request, CancellationToken ct = default)
    {
        var js = await Invoke("bffAuth.post", ct, "/bff/auth/password/forgot", request);
        return ParseNoData(js);
    }

    public async Task<BffResponse> ResetPasswordAsync(
        ResetPasswordRequest request, CancellationToken ct = default)
    {
        var js = await Invoke("bffAuth.post", ct, "/bff/auth/password/reset", request);
        return ParseNoData(js);
    }

    public async Task<BffResponse<AcceptInvitationResponse>> AcceptInvitationAsync(
        AcceptInvitationRequest request, CancellationToken ct = default)
    {
        var js = await Invoke("bffAuth.post", ct, "/bff/auth/invitations/accept", request);
        return Parse<AcceptInvitationResponse>(js);
    }

    public async Task ClearStoredTokenAsync()
    {
        try { await Js.InvokeVoidAsync("bffAuth.clearStoredToken"); }
        catch { }
    }

    // --- helpers ---

    private ValueTask<JsonElement> Invoke(string fn, CancellationToken ct, string url, object? body)
        => Js.InvokeAsync<JsonElement>(fn, ct, url, body);

    private static BffResponse<T> Parse<T>(JsonElement result)
    {
        var isSuccess = result.GetProperty("isSuccess").GetBoolean();
        var status = result.GetProperty("status").GetInt32();

        if (isSuccess)
        {
            var dataEl = result.GetProperty("data");
            T? data = dataEl.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
                ? default
                : dataEl.Deserialize<T>(JsonOpts);
            return new BffResponse<T> { IsSuccess = true, Status = status, Data = data };
        }

        var rawError = result.GetProperty("error").GetString() ?? "";
        return new BffResponse<T> { IsSuccess = false, Status = status, Error = ExtractError(rawError) };
    }

    private static BffResponse ParseNoData(JsonElement result)
    {
        var isSuccess = result.GetProperty("isSuccess").GetBoolean();
        var status = result.GetProperty("status").GetInt32();
        if (isSuccess) return new BffResponse { IsSuccess = true, Status = status };
        var rawError = result.GetProperty("error").GetString() ?? "";
        return new BffResponse { IsSuccess = false, Status = status, Error = ExtractError(rawError) };
    }

    private static string ExtractError(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Произошла ошибка.";
        try
        {
            var doc = JsonDocument.Parse(raw);
            if (doc.RootElement.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String)
                return t.GetString()!;
            if (doc.RootElement.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.String)
                return e.GetString()!;
        }
        catch (JsonException) { }
        return raw;
    }
}
