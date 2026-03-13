namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;

public record LoginResponse(string AccessToken, DateTime ExpiresAt, bool RequiresTwoFactor);