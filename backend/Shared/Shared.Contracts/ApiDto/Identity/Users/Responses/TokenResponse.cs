namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    bool RequiresTwoFactor = false);
