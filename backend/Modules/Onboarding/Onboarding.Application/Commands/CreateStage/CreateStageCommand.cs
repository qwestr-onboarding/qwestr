using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateStage;

public record CreateStageCommand(
    Guid TenantId,
    Guid ScenarioId,
    string Title,
    string? Description) : IRequest<ActionResult<Guid>>;
