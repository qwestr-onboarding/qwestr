namespace Qwestr.Modules.Identity.Application.DTOs;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    bool RequiresTwoFactor = false);
