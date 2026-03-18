using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateStep;

public class CreateStepCommandHandler : IRequestHandler<CreateStepCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public CreateStepCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateStepCommand request, CancellationToken ct)
    {
        var stage = await _scenarioRepository.GetStageByIdAsync(request.StageId, ct);
        if (stage is null)
        {
            return ActionResult<Guid>.Failure("Stage not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult<Guid>.Failure("Stage not found.");
        }

        var maxOrder = await _scenarioRepository.GetMaxStepOrderAsync(request.StageId, ct);
        var step = Step.Create(request.StageId, request.Title, request.Type, maxOrder + 1, request.Points, request.Content, request.Settings);

        await _scenarioRepository.AddStepAsync(step, ct);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(step.Id);
    }
}
