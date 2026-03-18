using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CompleteInformationStep;

public record CompleteInformationStepCommand(
    Guid TenantId,
    Guid UserId,
    Guid StepProgressId) : IRequest<ActionResult>;
