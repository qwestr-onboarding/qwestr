using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UnpublishScenario;

public record UnpublishScenarioCommand(
    Guid TenantId,
    Guid ScenarioId) : IRequest<ActionResult>;
