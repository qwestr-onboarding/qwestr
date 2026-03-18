using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveStage;

public record MoveStageCommand(
    Guid TenantId,
    Guid StageId,
    MoveDirection Direction) : IRequest<ActionResult>;
