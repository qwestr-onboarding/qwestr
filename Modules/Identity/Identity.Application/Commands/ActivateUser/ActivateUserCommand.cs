using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.ActivateUser;

public record ActivateUserCommand(Guid TenantId, Guid TargetUserId) : IRequest<ActionResult>;
