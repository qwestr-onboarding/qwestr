using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;

public record InvitationResponse(
    Guid Id,
    string Email,
    UserRole Role,
    InvitationStatus Status,
    string InvitedByName,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    string ActivationToken);
