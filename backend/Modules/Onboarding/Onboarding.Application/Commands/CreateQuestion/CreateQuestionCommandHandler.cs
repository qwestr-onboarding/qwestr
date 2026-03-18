using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateQuestion;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public CreateQuestionCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateQuestionCommand request, CancellationToken ct)
    {
        var step = await _scenarioRepository.GetStepByIdAsync(request.StepId, ct);
        if (step is null)
        {
            return ActionResult<Guid>.Failure("Step not found.");
        }

        var stage = await _scenarioRepository.GetStageByIdAsync(step.StageId, ct);
        if (stage is null)
        {
            return ActionResult<Guid>.Failure("Step not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult<Guid>.Failure("Step not found.");
        }

        if (step.Type == StepType.Question)
        {
            var count = await _scenarioRepository.CountQuestionsInStepAsync(request.StepId, ct);
            if (count >= 1)
            {
                return ActionResult<Guid>.Failure("A Question step can have at most 1 question.");
            }
        }

        var maxOrder = await _scenarioRepository.GetMaxQuestionOrderAsync(request.StepId, ct);
        var question = Question.Create(request.StepId, request.Type, request.QuestionText, maxOrder + 1);

        await _scenarioRepository.AddQuestionAsync(question, ct);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(question.Id);
    }
}
