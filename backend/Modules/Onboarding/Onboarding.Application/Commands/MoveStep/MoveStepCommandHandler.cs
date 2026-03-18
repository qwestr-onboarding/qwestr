using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveStep;

public class MoveStepCommandHandler : IRequestHandler<MoveStepCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public MoveStepCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(MoveStepCommand request, CancellationToken ct)
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

        var adjacent = await _scenarioRepository.GetAdjacentStepAsync(step.StageId, step.Order, request.Direction, ct);
        if (adjacent is null)
        {
            return ActionResult.Failure("Cannot move step in that direction.");
        }

        var currentOrder = step.Order;
        step.SetOrder(adjacent.Order);
        adjacent.SetOrder(currentOrder);

        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
