namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Responses;

public record RegisterResponse(Guid TenantId, string AccessToken, DateTime ExpiresAt);