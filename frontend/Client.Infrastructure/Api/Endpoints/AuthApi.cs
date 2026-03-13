using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class AuthApi(QwestrApiClient api)
{
    public Task<ApiResult<RegisterResponse>> RegisterAsync(
        RegisterTenantRequest request, CancellationToken ct = default)
        => api.PostAsync<RegisterResponse>("/api/identity/auth/register", request, ct);

    public Task<ApiResult<LoginResponse>> LoginAsync(
        LoginRequest request, CancellationToken ct = default)
        => api.PostAsync<LoginResponse>("/api/identity/auth/login", request, ct);

    public Task<ApiResult<VerifyTwoFactorResponse>> VerifyTwoFactorAsync(
        VerifyTwoFactorRequest factorRequest, CancellationToken ct = default)
        => api.PostAsync<VerifyTwoFactorResponse>("/api/identity/auth/login/2fa", factorRequest, ct);

    public Task<ApiResult<RefreshResponse>> RefreshAsync(CancellationToken ct = default)
        => api.PostAsync<RefreshResponse>("/api/identity/auth/refresh", new { }, ct);

    public Task<ApiResult> LogoutAsync(CancellationToken ct = default)
        => api.PostAsync("/api/identity/auth/logout", new { }, ct);

    public Task<ApiResult> ForgotPasswordAsync(
        RequestPasswordResetRequest request, CancellationToken ct = default)
        => api.PostAsync("/api/identity/auth/password/forgot", request, ct);

    public Task<ApiResult> ResetPasswordAsync(
        ResetPasswordRequest request, CancellationToken ct = default)
        => api.PostAsync("/api/identity/auth/password/reset", request, ct);
}