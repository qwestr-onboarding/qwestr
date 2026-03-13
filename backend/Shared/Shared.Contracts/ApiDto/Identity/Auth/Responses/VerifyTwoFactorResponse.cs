namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;

public record VerifyTwoFactorResponse(string AccessToken, DateTime ExpiresAt);