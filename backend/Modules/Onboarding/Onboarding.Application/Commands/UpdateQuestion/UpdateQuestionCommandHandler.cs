using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.UpdateQuestion;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public UpdateQuestionCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(UpdateQuestionCommand request, CancellationToken ct)
    {
        var question = await _scenarioRepository.GetQuestionByIdAsync(request.QuestionId, ct);
        if (question is null)
        {
            return ActionResult.Failure("Question not found.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(question.StepId, ct);
        if (step is null)
        {
            return ActionResult.Failure("Question not found.");
        }

        var stage = await _scenarioRepository.GetStageByIdAsync(step.StageId, ct);
        if (stage is null)
        {
            return ActionResult.Failure("Question not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Question not found.");
        }

        question.Update(request.Type, request.QuestionText);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
