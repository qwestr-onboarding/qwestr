namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;

public record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt,
    bool RequiresTwoFactor = false);
