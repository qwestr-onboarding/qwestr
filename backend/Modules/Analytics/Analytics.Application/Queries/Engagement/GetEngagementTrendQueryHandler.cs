using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Engagement;

public class GetEngagementTrendQueryHandler
    : IRequestHandler<GetEngagementTrendQuery, ActionResult<EngagementTrendResponse>>
{
    private readonly IDailyEngagementSnapshotRepository _repo;

    public GetEngagementTrendQueryHandler(IDailyEngagementSnapshotRepository repo)
    {
        _repo = repo;
    }

    public async Task<ActionResult<EngagementTrendResponse>> Handle(
        GetEngagementTrendQuery request, CancellationToken ct)
    {
        var days = Math.Clamp(request.Days, 1, 365);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = today.AddDays(-(days - 1));

        var snapshots = await _repo.GetRangeAsync(request.TenantId, from, today, ct);

        var points = snapshots
            .OrderBy(s => s.Date)
            .Select(s => new EngagementPointResponse(s.Date, s.ActiveUsers, s.StepsCompleted))
            .ToList();

        return ActionResult<EngagementTrendResponse>.Success(new EngagementTrendResponse(points));
    }
}
