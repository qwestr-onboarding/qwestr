using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Commands.CreateLevelTier;

public record CreateLevelTierCommand(
    Guid TenantId,
    int Order,
    string Title,
    int FromLevel,
    int ToLevel,
    string Color,
    string? FrameUrl) : IRequest<ActionResult<Guid>>;
