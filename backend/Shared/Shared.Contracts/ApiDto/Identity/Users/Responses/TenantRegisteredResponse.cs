namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;

public record TenantRegisteredResponse(
    Guid TenantId,
    string AccessToken,
    DateTime ExpiresAt);
