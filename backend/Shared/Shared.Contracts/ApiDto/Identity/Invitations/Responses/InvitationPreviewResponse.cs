using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;

public record InvitationPreviewResponse(string Email, UserRole Role, string CompanyName);