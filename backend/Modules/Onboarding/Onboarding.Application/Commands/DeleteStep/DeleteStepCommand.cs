using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteStep;

public record DeleteStepCommand(
    Guid TenantId,
    Guid StepId) : IRequest<ActionResult>;
