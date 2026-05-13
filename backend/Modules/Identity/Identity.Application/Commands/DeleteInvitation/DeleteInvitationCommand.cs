using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.DeleteInvitation;

public record DeleteInvitationCommand(Guid TenantId, Guid InvitationId) : IRequest<ActionResult>;
