using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Retention;

public class GetCohortRetentionQueryHandler
    : IRequestHandler<GetCohortRetentionQuery, ActionResult<CohortRetentionResponse>>
{
    private readonly IScenarioAssignmentFactRepository _repo;

    public GetCohortRetentionQueryHandler(IScenarioAssignmentFactRepository repo)
    {
        _repo = repo;
    }

    public async Task<ActionResult<CohortRetentionResponse>> Handle(
        GetCohortRetentionQuery request, CancellationToken ct)
    {
        var weeks = Math.Clamp(request.Weeks, 1, 52);
        var cohorts = await _repo.GetCohortRetentionAsync(request.TenantId, weeks, ct);

        var rows = cohorts.Select(c =>
        {
            var r1 = c.CohortSize > 0
                ? Math.Round((decimal)c.ActiveAfterWeek1 / c.CohortSize, 4)
                : 0m;
            var r4 = c.CohortSize > 0
                ? Math.Round((decimal)c.ActiveAfterWeek4 / c.CohortSize, 4)
                : 0m;
            return new CohortRowResponse(c.CohortWeekStart, c.CohortSize, r1, r4);
        }).ToList();

        return ActionResult<CohortRetentionResponse>.Success(new CohortRetentionResponse(rows));
    }
}
