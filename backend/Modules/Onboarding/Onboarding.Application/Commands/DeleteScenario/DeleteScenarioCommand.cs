using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteScenario;

public record DeleteScenarioCommand(
    Guid TenantId,
    Guid ScenarioId) : IRequest<ActionResult>;
