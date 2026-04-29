using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Awards.Commands.UpdateAward;

public record UpdateAwardCommand(Guid TenantId, Guid Id, string Title, string Description, string IconUrl) : IRequest<ActionResult>;
