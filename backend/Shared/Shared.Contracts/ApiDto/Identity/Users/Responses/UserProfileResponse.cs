using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;

public record UserProfileResponse(
    Guid Id,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    UserRole Role,
    bool IsActive,
    bool TwoFactorEnabled,
    DateTime? LastLoginAt,
    DateTime CreatedAt);
