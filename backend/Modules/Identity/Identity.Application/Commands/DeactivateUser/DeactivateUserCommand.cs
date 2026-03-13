using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid TenantId, Guid TargetUserId) : IRequest<ActionResult>;
