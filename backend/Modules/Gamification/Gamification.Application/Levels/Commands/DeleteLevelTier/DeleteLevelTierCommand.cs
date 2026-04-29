using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Commands.DeleteLevelTier;

public record DeleteLevelTierCommand(Guid TenantId, Guid Id) : IRequest<ActionResult>;
