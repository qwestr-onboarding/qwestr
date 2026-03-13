using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Commands.CancelInvitation;

public record CancelInvitationCommand(Guid TenantId, Guid InvitationId) : IRequest<ActionResult>;
