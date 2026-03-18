using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.MoveQuestionOption;

public class MoveQuestionOptionCommandHandler : IRequestHandler<MoveQuestionOptionCommand, ActionResult>
{
    private readonly IScenarioRepository _scenarioRepository;

    public MoveQuestionOptionCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult> Handle(MoveQuestionOptionCommand request, CancellationToken ct)
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

        var adjacent = await _scenarioRepository.GetAdjacentOptionAsync(option.QuestionId, option.Order, request.Direction, ct);
        if (adjacent is null)
        {
            return ActionResult.Failure("Cannot move option in that direction.");
        }

        var currentOrder = option.Order;
        option.SetOrder(adjacent.Order);
        adjacent.SetOrder(currentOrder);

        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult.Success();
    }
}
