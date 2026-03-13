using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Commands.CreateInvitation;

public record CreateInvitationCommand(
    Guid TenantId,
    Guid InvitedByUserId,
    string Email,
    UserRole Role) : IRequest<ActionResult<Guid>>;
