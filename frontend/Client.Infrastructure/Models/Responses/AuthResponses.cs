namespace Client.Infrastructure.Models.Responses;

public record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt,
    bool RequiresTwoFactor);

public record TenantRegisteredResponse(
    Guid TenantId,
    string AccessToken,
    DateTime ExpiresAt);