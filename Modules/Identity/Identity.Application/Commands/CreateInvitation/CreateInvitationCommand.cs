using MediatR;
using Qwestr.Modules.Identity.Domain.Enums;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.CreateInvitation;

public record CreateInvitationCommand(
    Guid TenantId,
    Guid InvitedByUserId,
    string Email,
    UserRole Role) : IRequest<ActionResult<Guid>>;
