using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.Identity;

public record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string FirstName,
    string LastName,
    string? Username,
    UserRole Role,
    bool IsActive);