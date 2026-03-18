using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.StartStep;

public record StartStepCommand(
    Guid TenantId,
    Guid UserId,
    Guid AssignmentId,
    Guid StepId) : IRequest<ActionResult>;
