using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Levels.Commands.UpdateLevelTier;

public record UpdateLevelTierCommand(
    Guid TenantId,
    Guid Id,
    string Title,
    int FromLevel,
    int ToLevel,
    string Color,
    string? FrameUrl) : IRequest<ActionResult>;
