using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CompleteTaskStep;

public record CompleteTaskStepCommand(
    Guid TenantId,
    Guid UserId,
    Guid StepProgressId) : IRequest<ActionResult>;
