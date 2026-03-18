using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.AssignScenario;

public record AssignScenarioCommand(
    Guid TenantId,
    Guid ScenarioId,
    Guid UserId,
    Guid AssignedByUserId) : IRequest<ActionResult<Guid>>;
