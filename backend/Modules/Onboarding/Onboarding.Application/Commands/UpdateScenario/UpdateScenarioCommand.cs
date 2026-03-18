using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateScenario;

public record UpdateScenarioCommand(
    Guid TenantId,
    Guid ScenarioId,
    string Title,
    string? Description,
    List<string> Tags) : IRequest<ActionResult>;
