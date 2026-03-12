using Qwestr.Modules.Identity.Domain.Enums;

namespace Qwestr.Modules.Identity.Application.DTOs;

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
