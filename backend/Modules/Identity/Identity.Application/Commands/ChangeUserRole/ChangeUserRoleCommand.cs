using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(
    Guid TenantId,
    Guid CurrentUserId,
    Guid TargetUserId,
    UserRole NewRole) : IRequest<ActionResult>;
