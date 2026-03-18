using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.DuplicateScenario;

public class DuplicateScenarioCommandHandler : IRequestHandler<DuplicateScenarioCommand, ActionResult<Guid>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public DuplicateScenarioCommandHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<Guid>> Handle(DuplicateScenarioCommand request, CancellationToken ct)
    {
        var source = await _scenarioRepository.GetWithFullTreeAsync(request.ScenarioId, ct);
        if (source is null || source.TenantId != request.TenantId)
        {
            return ActionResult<Guid>.Failure("Scenario not found.");
        }

        var copy = Scenario.Create(
            request.TenantId,
            source.Title + " (копия)",
            source.Description,
            new List<string>(source.Tags));

        foreach (var sourceStage in source.Stages.OrderBy(s => s.Order))
        {
            var stageCopy = Stage.Create(copy.Id, sourceStage.Title, sourceStage.Description, sourceStage.Order);

            foreach (var sourceStep in sourceStage.Steps.OrderBy(s => s.Order))
            {
                var stepCopy = Step.Create(
                    stageCopy.Id,
                    sourceStep.Title,
                    sourceStep.Type,
                    sourceStep.Order,
                    sourceStep.Points,
                    sourceStep.Content,
                    sourceStep.Settings);

                foreach (var sourceQuestion in sourceStep.Questions.OrderBy(q => q.Order))
                {
                    var questionCopy = Question.Create(stepCopy.Id, sourceQuestion.Type, sourceQuestion.QuestionText, sourceQuestion.Order);

                    foreach (var sourceOption in sourceQuestion.Options.OrderBy(o => o.Order))
                    {
                        var optionCopy = QuestionOption.Create(questionCopy.Id, sourceOption.Text, sourceOption.IsCorrect, sourceOption.Order);
                        await _scenarioRepository.AddOptionAsync(optionCopy, ct);
                    }

                    await _scenarioRepository.AddQuestionAsync(questionCopy, ct);
                }

                await _scenarioRepository.AddStepAsync(stepCopy, ct);
            }

            await _scenarioRepository.AddStageAsync(stageCopy, ct);
        }

        await _scenarioRepository.AddAsync(copy, ct);
        await _scenarioRepository.SaveChangesAsync(ct);

        return ActionResult<Guid>.Success(copy.Id);
    }
}
