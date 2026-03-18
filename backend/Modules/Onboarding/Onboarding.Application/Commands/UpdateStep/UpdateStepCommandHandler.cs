using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateStep;

public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public UpdateStepCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(UpdateStepCommand request, CancellationToken ct)
    {
        var step = await _scenarioRepository.GetStepByIdAsync(request.StepId, ct);
        if (step is null)
        {
            return ActionResult.Failure("Step not found.");
        }

        var stage = await _scenarioRepository.GetStageByIdAsync(step.StageId, ct);
        if (stage is null)
        {
            return ActionResult.Failure("Step not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Step not found.");
        }

        step.Update(request.Title, request.Points, request.Content, request.Settings);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
