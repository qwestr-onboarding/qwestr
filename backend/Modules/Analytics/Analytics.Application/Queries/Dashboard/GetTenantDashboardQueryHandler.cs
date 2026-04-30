using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Dashboard;

public class GetTenantDashboardQueryHandler : IRequestHandler<GetTenantDashboardQuery, ActionResult<DashboardResponse>>
{
    private readonly IStepCompletionFactRepository _stepRepo;
    private readonly IScenarioAssignmentFactRepository _assignmentRepo;
    private readonly IDailyEngagementSnapshotRepository _snapshotRepo;
    private readonly IScenarioMetadataCacheRepository _metaRepo;
    private readonly IAnalyticsInsightSetRepository _insightRepo;

    public GetTenantDashboardQueryHandler(
        IStepCompletionFactRepository stepRepo,
        IScenarioAssignmentFactRepository assignmentRepo,
        IDailyEngagementSnapshotRepository snapshotRepo,
        IScenarioMetadataCacheRepository metaRepo,
        IAnalyticsInsightSetRepository insightRepo)
    {
        _stepRepo = stepRepo;
        _assignmentRepo = assignmentRepo;
        _snapshotRepo = snapshotRepo;
        _metaRepo = metaRepo;
        _insightRepo = insightRepo;
    }

    public async Task<ActionResult<DashboardResponse>> Handle(
        GetTenantDashboardQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from30 = today.AddDays(-29);

        var snapshots = await _snapshotRepo.GetRangeAsync(request.TenantId, from30, today, ct);
        var atRisk = await _stepRepo.CountAtRiskEmployeesAsync(request.TenantId, TimeSpan.FromDays(7), ct);
        var scenarioSummaries = await _assignmentRepo.GetActiveScenariosSummaryAsync(request.TenantId, ct);

        var scenarioIds = scenarioSummaries.Select(s => s.ScenarioId).ToList();
        var scenarioNames = await _metaRepo.GetScenarioNamesAsync(request.TenantId, scenarioIds, ct);

        var latestInsightSet = await _insightRepo.GetLatestAsync(request.TenantId, ct);

        var totalAssigned = scenarioSummaries.Sum(s => s.Assigned);
        var totalCompleted = scenarioSummaries.Sum(s => s.Completed);
        var avgCompletionRate = totalAssigned > 0
            ? Math.Round((decimal)totalCompleted / totalAssigned, 4)
            : 0m;

        var activeEmployees = await _snapshotRepo.CountDistinctActiveUsersAsync(request.TenantId, today, ct);
        var avgDuration = await _assignmentRepo.GetAvgCompletionDurationAsync(request.TenantId, ct);

        var kpis = new DashboardKpisResponse(activeEmployees, avgCompletionRate, avgDuration, atRisk);

        var trend = snapshots
            .OrderBy(s => s.Date)
            .Select(s => new EngagementPointResponse(s.Date, s.ActiveUsers, s.StepsCompleted))
            .ToList();

        var topProblemSteps = await BuildTopProblemStepsAsync(request.TenantId, scenarioIds, scenarioNames, ct);

        var activeScenarios = scenarioSummaries
            .Select(s => new ScenarioSummaryResponse(
                s.ScenarioId,
                scenarioNames.GetValueOrDefault(s.ScenarioId, "—"),
                s.Assigned,
                s.Completed,
                s.Assigned > 0 ? Math.Round((decimal)s.Completed / s.Assigned, 4) : 0m))
            .ToList();

        AnalyticsInsightSetResponse? insightsResponse = null;
        if (latestInsightSet is not null)
        {
            insightsResponse = MapInsightSet(latestInsightSet);
        }

        return ActionResult<DashboardResponse>.Success(
            new DashboardResponse(kpis, trend, topProblemSteps, activeScenarios, insightsResponse));
    }

    private async Task<IReadOnlyList<ProblemStepResponse>> BuildTopProblemStepsAsync(
        Guid tenantId, IEnumerable<Guid> scenarioIds, IReadOnlyDictionary<Guid, string> scenarioNames, CancellationToken ct)
    {
        var result = new List<(ProblemStepResponse dto, decimal dropOff)>();

        foreach (var scenarioId in scenarioIds)
        {
            var stats = await _stepRepo.GetStepStatsAsync(tenantId, scenarioId, ct);
            var stepIds = stats.Select(s => s.StepId).ToList();
            var stepNames = await _metaRepo.GetStepNamesAsync(tenantId, stepIds, ct);
            var scenarioName = scenarioNames.GetValueOrDefault(scenarioId, "—");

            foreach (var s in stats)
            {
                if (s.Attempts == 0)
                {
                    continue;
                }

                var dropOff = s.UniqueUsers > 0
                    ? 1m - (decimal)s.Successful / s.UniqueUsers
                    : 0m;

                var passRate = s.Attempts > 0 && s.AvgScoreRatio.HasValue
                    ? s.AvgScoreRatio
                    : (s.Attempts > 0 ? (decimal?)s.FirstAttemptPassCount / s.Attempts : null);

                if (dropOff > 0.1m)
                {
                    result.Add((new ProblemStepResponse(
                        s.StepId,
                        stepNames.GetValueOrDefault(s.StepId, "—"),
                        scenarioName,
                        Math.Round(dropOff, 4),
                        passRate.HasValue ? Math.Round(passRate.Value, 4) : null),
                        dropOff));
                }
            }
        }

        return result
            .OrderByDescending(x => x.dropOff)
            .Take(5)
            .Select(x => x.dto)
            .ToList();
    }

    private static AnalyticsInsightSetResponse MapInsightSet(Domain.Entities.Insights.AnalyticsInsightSet set)
    {
        var insights = set.Insights
            .Select(i => new InsightResponse(i.Title, i.Description, i.Severity, i.MetricReference))
            .ToList();
        var recs = set.Recommendations
            .Select(r => new RecommendationResponse(r.Title, r.Description, r.Category, r.Priority))
            .ToList();
        return new AnalyticsInsightSetResponse(set.Id, set.GeneratedAt, set.Provider, set.Trigger, insights, recs);
    }
}
