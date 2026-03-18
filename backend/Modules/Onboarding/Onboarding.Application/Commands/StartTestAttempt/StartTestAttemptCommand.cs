using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.StartTestAttempt;

public record StartTestAttemptCommand(
    Guid TenantId,
    Guid UserId,
    Guid StepProgressId) : IRequest<ActionResult<Guid>>;
