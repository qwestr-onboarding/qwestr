using Qwestr.Modules.Identity.Domain.Enums;

namespace Qwestr.Modules.Identity.Application.DTOs;

public record UserListItemResponse(
    Guid Id,
    string Email,
    string FullName,
    UserRole Role,
    bool IsActive,
    DateTime? LastLoginAt);
