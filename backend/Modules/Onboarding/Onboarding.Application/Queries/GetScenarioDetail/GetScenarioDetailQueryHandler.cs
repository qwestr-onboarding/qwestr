using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetScenarioDetail;

public class GetScenarioDetailQueryHandler : IRequestHandler<GetScenarioDetailQuery, ActionResult<ScenarioDetailResponse>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public GetScenarioDetailQueryHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<ScenarioDetailResponse>> Handle(GetScenarioDetailQuery request, CancellationToken ct)
    {
        var scenario = await _scenarioRepository.GetWithFullTreeAsync(request.ScenarioId, ct);
        if (scenario is null || scenario.TenantId != request.TenantId)
        {
            return ActionResult<ScenarioDetailResponse>.Failure("Scenario not found.");
        }

        var stages = scenario.Stages.OrderBy(s => s.Order).Select(stage => new StageResponse(
            stage.Id,
            stage.Title,
            stage.Description,
            stage.Order,
            stage.Steps.OrderBy(st => st.Order).Select(step => new StepResponse(
                step.Id,
                step.Title,
                step.Type,
                step.Order,
                step.Points,
                step.Content,
                step.Settings,
                step.Questions.OrderBy(q => q.Order).Select(q => new QuestionResponse(
                    q.Id,
                    q.Type,
                    q.QuestionText,
                    q.Order,
                    q.Options.OrderBy(o => o.Order).Select(o => new QuestionOptionResponse(
                        o.Id,
                        o.Text,
                        o.IsCorrect,
                        o.Order)).ToList()
                )).ToList()
            )).ToList()
        )).ToList();

        var response = new ScenarioDetailResponse(
            scenario.Id,
            scenario.Title,
            scenario.Description,
            scenario.Tags,
            scenario.IsPublished,
            scenario.CreatedAt,
            scenario.UpdatedAt,
            stages);

        return ActionResult<ScenarioDetailResponse>.Success(response);
    }
}
