using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CompleteChecklistStep;

public record CompleteChecklistStepCommand(
    Guid TenantId,
    Guid UserId,
    Guid StepProgressId) : IRequest<ActionResult>;
