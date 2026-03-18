using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateScenario;

public class UpdateScenarioCommandHandler : IRequestHandler<UpdateScenarioCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public UpdateScenarioCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(UpdateScenarioCommand request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetByIdAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Scenario not found.");
        }

        scenario.Update(request.Title, request.Description, request.Tags);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
