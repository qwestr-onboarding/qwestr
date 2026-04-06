using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateStep;

public record UpdateStepCommand(
    Guid TenantId,
    Guid StepId,
    string Title,
    StepType Type,
    int? Points,
    string? Content,
    string? Settings) : IRequest<ActionResult>;
