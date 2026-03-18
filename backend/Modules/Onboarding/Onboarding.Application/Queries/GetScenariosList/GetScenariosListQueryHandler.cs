using MediatR;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetScenariosList;

public class GetScenariosListQueryHandler : IRequestHandler<GetScenariosListQuery, ActionResult<PagedList<ScenarioListItemResponse>>>
{
    private readonly IScenarioRepository _scenarioRepository;

    public GetScenariosListQueryHandler(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<ActionResult<PagedList<ScenarioListItemResponse>>> Handle(GetScenariosListQuery request, CancellationToken ct)
    {
        var scenarios = await _scenarioRepository.GetByTenantAsync(request.TenantId, request.Page, request.PageSize, request.TagFilter, ct);
        var total = await _scenarioRepository.CountByTenantAsync(request.TenantId, request.TagFilter, ct);

        var items = scenarios.Select(s => new ScenarioListItemResponse(
            s.Id,
            s.Title,
            s.Description,
            s.Tags,
            s.IsPublished,
            s.Stages.Count,
            s.Stages.Sum(st => st.Steps.Count),
            s.CreatedAt,
            s.UpdatedAt)).ToList();

        return ActionResult<PagedList<ScenarioListItemResponse>>.Success(new PagedList<ScenarioListItemResponse>(items, request.Page, request.PageSize, total));
    }
}
