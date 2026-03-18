using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateStep;

public record UpdateStepCommand(
    Guid TenantId,
    Guid StepId,
    string Title,
    int? Points,
    string? Content,
    string? Settings) : IRequest<ActionResult>;
