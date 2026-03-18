using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateChecklistProgress;

public record UpdateChecklistProgressCommand(
    Guid TenantId,
    Guid UserId,
    Guid StepProgressId,
    List<int> CheckedIndices) : IRequest<ActionResult>;
