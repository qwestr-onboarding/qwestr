using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateStage;

public class CreateStageCommandHandler : IRequestHandler<CreateStageCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public CreateStageCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateStageCommand request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetByIdAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult<Guid>.Failure("Scenario not found.");
        }

        var maxOrder = await _scenarioRepository.GetMaxStageOrderAsync(request.ScenarioId, ct);
        var stage = Stage.Create(request.ScenarioId, request.Title, request.Description, maxOrder + 1);

        await _scenarioRepository.AddStageAsync(stage, ct);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(stage.Id);
    }
}
