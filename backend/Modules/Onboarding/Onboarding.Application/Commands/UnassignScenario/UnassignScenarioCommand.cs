using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UnassignScenario;

public record UnassignScenarioCommand(
    Guid TenantId,
    Guid AssignmentId) : IRequest<ActionResult>;
