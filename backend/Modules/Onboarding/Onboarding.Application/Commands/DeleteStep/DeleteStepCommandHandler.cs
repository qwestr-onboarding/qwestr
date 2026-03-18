using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteStep;

public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public DeleteStepCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(DeleteStepCommand request, CancellationToken ct)
    {
        var step = await _scenarioRepository.GetStepWithQuestionsAsync(request.StepId, ct);
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

        foreach (var question in step.Questions)
        {
            foreach (var option in question.Options)
            {
                _scenarioRepository.RemoveOption(option);
            }
            _scenarioRepository.RemoveQuestion(question);
        }

        _scenarioRepository.RemoveStep(step);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
