using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateStep;

public record CreateStepCommand(
    Guid TenantId,
    Guid StageId,
    string Title,
    StepType Type,
    int? Points,
    string? Content,
    string? Settings) : IRequest<ActionResult<Guid>>;
