namespace Client.Infrastructure.Models.Responses;

public record UserProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Username,
    string Role,
    bool TwoFactorEnabled,
    DateTime? LastLoginAt,
    DateTime CreatedAt);

public record UserListItemResponse(
    Guid Id,
    string Email,
    string FullName,
    string? Username,
    string Role,
    bool IsActive,
    DateTime? LastLoginAt);

public record TwoFactorSetupResponse(string Secret, string QrCodeUri);

public record BackupCodesResponse(List<string> Codes);