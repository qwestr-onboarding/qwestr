using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UnpublishScenario;

public class UnpublishScenarioCommandHandler : IRequestHandler<UnpublishScenarioCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public UnpublishScenarioCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(UnpublishScenarioCommand request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetByIdAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Scenario not found.");
        }

        scenario.Unpublish();
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
