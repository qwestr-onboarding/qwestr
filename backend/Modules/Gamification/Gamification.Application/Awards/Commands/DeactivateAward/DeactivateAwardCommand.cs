using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.DeactivateAward;

public record DeactivateAwardCommand(Guid TenantId, Guid Id) : IRequest<ActionResult>;
