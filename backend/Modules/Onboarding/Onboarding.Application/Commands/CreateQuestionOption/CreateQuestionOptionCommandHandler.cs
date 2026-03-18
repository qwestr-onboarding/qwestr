using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.CreateQuestionOption;

public class CreateQuestionOptionCommandHandler : IRequestHandler<CreateQuestionOptionCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public CreateQuestionOptionCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(CreateQuestionOptionCommand request, CancellationToken ct)
    {
        var question = await _scenarioRepository.GetQuestionByIdAsync(request.QuestionId, ct);
        if (question is null)
        {
            return ActionResult<Guid>.Failure("Question not found.");
        }

        var step = await _scenarioRepository.GetStepByIdAsync(question.StepId, ct);
        if (step is null)
        {
            return ActionResult<Guid>.Failure("Question not found.");
        }

        var stage = await _scenarioRepository.GetStageByIdAsync(step.StageId, ct);
        if (stage is null)
        {
            return ActionResult<Guid>.Failure("Question not found.");
        }

        var scenario = await _scenarioRepository.GetByIdAsync(stage.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult<Guid>.Failure("Question not found.");
        }

        var maxOrder = await _scenarioRepository.GetMaxOptionOrderAsync(request.QuestionId, ct);
        var option = QuestionOption.Create(request.QuestionId, request.Text, request.IsCorrect, maxOrder + 1);

        await _scenarioRepository.AddOptionAsync(option, ct);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(option.Id);
    }
}
