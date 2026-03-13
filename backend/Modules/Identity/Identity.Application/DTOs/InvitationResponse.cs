using Qwestr.Modules.Identity.Domain.Enums;

namespace Qwestr.Modules.Identity.Application.DTOs;

public record InvitationResponse(
    Guid Id,
    string Email,
    UserRole Role,
    InvitationStatus Status,
    string InvitedByName,
    DateTime CreatedAt,
    DateTime ExpiresAt);
