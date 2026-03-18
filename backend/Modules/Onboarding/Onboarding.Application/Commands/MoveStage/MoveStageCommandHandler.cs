using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveStage;

public class MoveStageCommandHandler : IRequestHandler<MoveStageCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public MoveStageCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(MoveStageCommand request, CancellationToken ct)
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

        var adjacent = await _scenarioRepository.GetAdjacentStageAsync(stage.ScenarioId, stage.Order, request.Direction, ct);
        if (adjacent is null)
        {
            return ActionResult.Failure("Cannot move stage in that direction.");
        }

        var currentOrder = stage.Order;
        stage.SetOrder(adjacent.Order);
        adjacent.SetOrder(currentOrder);

        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
