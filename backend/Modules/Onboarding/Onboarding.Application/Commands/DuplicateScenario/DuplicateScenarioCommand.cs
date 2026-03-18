using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DuplicateScenario;

public record DuplicateScenarioCommand(
    Guid TenantId,
    Guid ScenarioId,
    Guid CreatedByUserId) : IRequest<ActionResult<Guid>>;
