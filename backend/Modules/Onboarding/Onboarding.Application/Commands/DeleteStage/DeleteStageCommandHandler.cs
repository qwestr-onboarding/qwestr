using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteStage;

public class DeleteStageCommandHandler : IRequestHandler<DeleteStageCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public DeleteStageCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(DeleteStageCommand request, CancellationToken ct)
    {
        var stage = await _scenarioRepository.GetStageWithStepsAsync(request.StageId, ct);
        if (stage is null)
        {
            return ActionResult.Failure("Stage not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Stage not found.");
        }

        foreach (var step in stage.Steps)
        {
            foreach (var question in step.Questions)
            {
                foreach (var option in question.Options)
                {
                    _scenarioRepository.RemoveOption(option);
                }
                _scenarioRepository.RemoveQuestion(question);
            }
            _scenarioRepository.RemoveStep(step);
        }

        _scenarioRepository.RemoveStage(stage);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
