using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.PublishScenario;

public record PublishScenarioCommand(
    Guid TenantId,
    Guid ScenarioId) : IRequest<ActionResult>;
