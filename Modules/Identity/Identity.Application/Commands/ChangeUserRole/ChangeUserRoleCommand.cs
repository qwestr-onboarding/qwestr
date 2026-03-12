using MediatR;
using Qwestr.Modules.Identity.Domain.Enums;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(
    Guid TenantId,
    Guid CurrentUserId,
    Guid TargetUserId,
    UserRole NewRole) : IRequest<ActionResult>;
