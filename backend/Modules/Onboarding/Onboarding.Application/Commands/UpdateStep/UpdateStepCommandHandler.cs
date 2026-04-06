using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

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

        if (request.Type == StepType.Question)
        {
            var extraQuestions = step.Questions.OrderBy(q => q.Order).Skip(1).ToList();
            foreach (var question in extraQuestions)
            {
                _scenarioRepository.RemoveQuestion(question);
            }
        }
        else if (request.Type != StepType.Test)
        {
            foreach (var question in step.Questions.ToList())
            {
                _scenarioRepository.RemoveQuestion(question);
            }
        }

        step.Update(request.Title, request.Type, request.Points, request.Content, request.Settings);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
