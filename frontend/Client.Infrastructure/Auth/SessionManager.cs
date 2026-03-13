using System.Net;
using Client.Infrastructure.Api.Models;
using Microsoft.AspNetCore.Components;

namespace Client.Infrastructure.Auth;

public class SessionManager
{
    private readonly TokenStorageService _tokenStorage;
    private readonly BffAuthClient _bffAuth;
    private readonly JwtAuthStateProvider _authState;
    private readonly NavigationManager _nav;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public SessionManager(
        TokenStorageService tokenStorage,
        BffAuthClient bffAuth,
        JwtAuthStateProvider authState,
        NavigationManager nav)
    {
        _tokenStorage = tokenStorage;
        _bffAuth = bffAuth;
        _authState = authState;
        _nav = nav;
    }

    /// <summary>
    /// Гарантирует валидность access token. Делает refresh если нужно.
    /// Возвращает false и редиректит на /login если сессия потеряна.
    /// </summary>
    public async Task<bool> EnsureValidTokenAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
            return true;

        await _refreshLock.WaitAsync(ct);
        try
        {
            // Повторная проверка — другой вызов мог уже обновить токен
            if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
                return true;

            var result = await _bffAuth.RefreshAsync(ct);
            if (result.IsSuccess && result.Data is not null)
            {
                _tokenStorage.SetToken(result.Data.AccessToken, result.Data.ExpiresAt);
                _authState.NotifyAuthStateChanged();
                return true;
            }

            _tokenStorage.Clear();
            _authState.NotifyAuthStateChanged();
            _nav.NavigateTo("/login");
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    /// <summary>
    /// Пытается восстановить сессию через refresh cookie.
    /// В отличие от EnsureValidTokenAsync — НЕ редиректит на /login при неудаче.
    /// Используется на auth-страницах: если сессия жива — перенаправить на /dashboard.
    /// </summary>
    public async Task<bool> TryRestoreSessionAsync(CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
            return true;

        await _refreshLock.WaitAsync(ct);
        try
        {
            if (!string.IsNullOrEmpty(_tokenStorage.AccessToken) && !_tokenStorage.IsExpired)
                return true;

            var result = await _bffAuth.RefreshAsync(ct);
            if (result.IsSuccess && result.Data is not null)
            {
                _tokenStorage.SetToken(result.Data.AccessToken, result.Data.ExpiresAt);
                _authState.NotifyAuthStateChanged();
                return true;
            }

            _tokenStorage.Clear();
            return false;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    /// <summary>
    /// Выполняет авторизованный API-вызов. При 401 — делает refresh и повторяет.
    /// </summary>
    public async Task<ApiResult<T>> ExecuteWithAuthAsync<T>(
        Func<Task<ApiResult<T>>> apiCall, CancellationToken ct = default)
    {
        if (!await EnsureValidTokenAsync(ct))
            return ApiResult<T>.Failure(HttpStatusCode.Unauthorized, "Session expired");

        var result = await apiCall();

        if (result.StatusCode != HttpStatusCode.Unauthorized)
            return result;

        // Токен был "живой" по нашим данным, но backend отклонил (рассинхрон / отзыв)
        _tokenStorage.Clear();

        if (!await EnsureValidTokenAsync(ct))
            return ApiResult<T>.Failure(HttpStatusCode.Unauthorized, "Session expired");

        return await apiCall();
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

        if (result.StatusCode != HttpStatusCode.Unauthorized)
            return result;

        _tokenStorage.Clear();

        if (!await EnsureValidTokenAsync(ct))
            return ApiResult.Failure(HttpStatusCode.Unauthorized, "Session expired");

        return await apiCall();
    }
}