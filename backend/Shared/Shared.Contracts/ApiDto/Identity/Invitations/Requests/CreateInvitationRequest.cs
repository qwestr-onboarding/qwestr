using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Requests;

public record CreateInvitationRequest(string Email, UserRole Role);
