using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateScenario;

public record CreateScenarioCommand(
    Guid TenantId,
    Guid CreatedByUserId,
    string Title,
    string? Description,
    List<string> Tags) : IRequest<ActionResult<Guid>>;
