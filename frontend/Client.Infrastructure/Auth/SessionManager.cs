using System.Net;
using System.Text.Json;
using Client.Infrastructure.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Client.Infrastructure.Auth;

public class SessionManager
{
    private const string CookieName = "qwestr_at";

    private readonly TokenStorageService _tokenStorage;
    private readonly BffAuthClient _bffAuth;
    private readonly JwtAuthStateProvider _authState;
    private readonly NavigationManager _nav;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionManager(
        TokenStorageService tokenStorage,
        BffAuthClient bffAuth,
        JwtAuthStateProvider authState,
        NavigationManager nav,
        IHttpContextAccessor httpContextAccessor)
    {
        _tokenStorage = tokenStorage;
        _bffAuth = bffAuth;
        _authState = authState;
        _nav = nav;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Пытается восстановить сессию из HttpOnly cookie через HttpContext.
    /// НЕ редиректит на /login при неудаче — используется на auth-страницах.
    /// </summary>
    public Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
            return Task.FromResult(true);

        var stored = ReadTokenFromCookie();
        if (stored is null)
            return Task.FromResult(false);

        _tokenStorage.SetToken(stored.Value.Token, stored.Value.ExpiresAt);
        _authState.NotifyAuthStateChanged();
        return Task.FromResult(true);
    }

    /// <summary>
    /// Гарантирует валидный токен. Редиректит на /login если сессия потеряна.
    /// </summary>
    public async Task<bool> EnsureValidTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
            return true;

        if (await TryRestoreSessionAsync(ct))
            return true;

        _tokenStorage.Clear();
        _authState.NotifyAuthStateChanged();
        _nav.NavigateTo("/login");
        return false;
    }

    /// <summary>
    /// Выполняет авторизованный API-вызов. При 401 — очищает сессию и редиректит.
    /// </summary>
    public async Task<ApiResult<T>> ExecuteWithAuthAsync<T>(
        Func<Task<ApiResult<T>>> apiCall, CancellationToken ct = default)
    {
        if (!await EnsureValidTokenAsync(ct))
            return ApiResult<T>.Failure(HttpStatusCode.Unauthorized, "Session expired");

        var result = await apiCall();

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            _tokenStorage.Clear();
            await _bffAuth.ClearStoredTokenAsync();
            _authState.NotifyAuthStateChanged();
            _nav.NavigateTo("/login");
        }

        return result;
    }

    /// <summary>
    /// Перегрузка для API-вызовов без тела ответа.
    /// </summary>
    public async Task<ApiResult> ExecuteWithAuthAsync(
        Func<Task<ApiResult>> apiCall, CancellationToken ct = default)
    {
        if (!await EnsureValidTokenAsync(ct))
            return ApiResult.Failure(HttpStatusCode.Unauthorized, "Session expired");

        var result = await apiCall();

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            _tokenStorage.Clear();
            await _bffAuth.ClearStoredTokenAsync();
            _authState.NotifyAuthStateChanged();
            _nav.NavigateTo("/login");
        }

        return result;
    }

    /// <summary>
    /// Сбрасывает текущую сессию.
    /// </summary>
    public void ClearSession()
    {
        _tokenStorage.Clear();
        _authState.NotifyAuthStateChanged();
    }

    /// <summary>
    /// Читает claim из текущего access token.
    /// </summary>
    public string? GetClaimFromToken(string claimType)
    {
        var token = _tokenStorage.AccessToken;
        if (string.IsNullOrEmpty(token)) return null;
        return _authState.ParseClaimsFromJwt(token)
            .FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    private (string Token, DateTime ExpiresAt)? ReadTokenFromCookie()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];
        if (string.IsNullOrEmpty(token)) return null;

        try
        {
            var parts = token.Split('.');
            if (parts.Length < 2) return null;
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            var padded = payload.PadRight((payload.Length + 3) & ~3, '=');
            using var doc = JsonDocument.Parse(Convert.FromBase64String(padded));
            var exp = doc.RootElement.GetProperty("exp").GetInt64();
            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            if (expiresAt <= DateTime.UtcNow) return null;
            return (token, expiresAt);
        }
        catch { return null; }
    }
}
