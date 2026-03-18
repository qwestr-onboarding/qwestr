using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveStep;

public record MoveStepCommand(
    Guid TenantId,
    Guid StepId,
    MoveDirection Direction) : IRequest<ActionResult>;
