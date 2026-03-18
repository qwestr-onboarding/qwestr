using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateStage;

public class UpdateStageCommandHandler : IRequestHandler<UpdateStageCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public UpdateStageCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(UpdateStageCommand request, CancellationToken ct)
    {
        var stage = await _scenarioRepository.GetStageByIdAsync(request.StageId, ct);
        if (stage is null)
        {
            return ActionResult.Failure("Stage not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Stage not found.");
        }

        stage.Update(request.Title, request.Description);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
