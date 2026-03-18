using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateStage;

public record UpdateStageCommand(
    Guid TenantId,
    Guid StageId,
    string Title,
    string? Description) : IRequest<ActionResult>;
