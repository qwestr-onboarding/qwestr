namespace Qwestr.Modules.Identity.Application.DTOs;

public record TenantRegisteredResponse(
    Guid TenantId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
