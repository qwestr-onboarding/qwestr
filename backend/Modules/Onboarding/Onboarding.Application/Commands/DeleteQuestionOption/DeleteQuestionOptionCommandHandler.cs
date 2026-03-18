using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DeleteQuestionOption;

public class DeleteQuestionOptionCommandHandler : IRequestHandler<DeleteQuestionOptionCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public DeleteQuestionOptionCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(DeleteQuestionOptionCommand request, CancellationToken ct)
    {
        var option = await _scenarioRepository.GetOptionByIdAsync(request.OptionId, ct);
        if (option is null)
        {
            return ActionResult.Failure("Option not found.");
        }

        var question = await _scenarioRepository.GetQuestionByIdAsync(option.QuestionId, ct);
        if (question is null)
        {
            return ActionResult.Failure("Option not found.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(question.StepId, ct);
        if (step is null)
        {
            return ActionResult.Failure("Option not found.");
        }

        var stage = await _scenarioRepository.GetStageByIdAsync(step.StageId, ct);
        if (stage is null)
        {
            return ActionResult.Failure("Option not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult.Failure("Option not found.");
        }

        _scenarioRepository.RemoveOption(option);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
