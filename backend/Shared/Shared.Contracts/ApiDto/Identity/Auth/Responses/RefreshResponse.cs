namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;

public record RefreshResponse(string AccessToken, DateTime ExpiresAt);