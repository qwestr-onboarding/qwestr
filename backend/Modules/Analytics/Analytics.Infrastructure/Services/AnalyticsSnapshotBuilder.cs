using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Models;

namespace Qwestr.Modules.Analytics.Infrastructure.Services;

public class AnalyticsSnapshotBuilder : IAnalyticsSnapshotBuilder
{
    private readonly IScenarioAssignmentFactRepository _assignmentRepo;
    private readonly IStepCompletionFactRepository _stepRepo;
    private readonly IScenarioMetadataCacheRepository _metaRepo;

    public AnalyticsSnapshotBuilder(
        IScenarioAssignmentFactRepository assignmentRepo,
        IStepCompletionFactRepository stepRepo,
        IScenarioMetadataCacheRepository metaRepo)
    {
        _assignmentRepo = assignmentRepo;
        _stepRepo = stepRepo;
        _metaRepo = metaRepo;
    }

    public async Task<AnalyticsSnapshot> BuildAsync(Guid tenantId, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from30 = today.AddDays(-29);

        var scenarioSummaries = await _assignmentRepo.GetActiveScenariosSummaryAsync(tenantId, ct);
        var scenarioIds = scenarioSummaries.Select(s => s.ScenarioId).ToList();
        var scenarioNames = await _metaRepo.GetScenarioNamesAsync(tenantId, scenarioIds, ct);

        var totalAssigned = scenarioSummaries.Sum(s => s.Assigned);
        var totalCompleted = scenarioSummaries.Sum(s => s.Completed);
        var avgCompletionRate = totalAssigned > 0
            ? Math.Round((decimal)totalCompleted / totalAssigned, 4)
            : 0m;

        var atRisk = await _stepRepo.CountAtRiskEmployeesAsync(tenantId, TimeSpan.FromDays(7), ct);
        var activeEmployees = await _assignmentRepo.CountActiveEmployeesAsync(tenantId, ct);
        var avgDuration = await _assignmentRepo.GetAvgCompletionDurationAsync(tenantId, ct);

        var overall = new OverallMetrics(
            ActiveEmployees: activeEmployees,
            TotalAssignments: totalAssigned,
            AvgCompletionRate: avgCompletionRate,
            AvgOnboardingDurationDays: (int)avgDuration.TotalDays,
            AtRiskEmployees: atRisk);

        var scenarioMetrics = scenarioSummaries.Select(s =>
        {
            var name = scenarioNames.GetValueOrDefault(s.ScenarioId, "—");
            var rate = s.Assigned > 0 ? Math.Round((decimal)s.Completed / s.Assigned, 4) : 0m;
            return new ScenarioMetric(name, s.Assigned, s.Completed, rate, 0);
        }).ToList();

        var problemSteps = new List<ProblemStepSnapshot>();
        foreach (var scenarioId in scenarioIds)
        {
            var stats = await _stepRepo.GetStepStatsAsync(tenantId, scenarioId, ct);
            var stepIds = stats.Select(s => s.StepId).ToList();
            var stepNames = await _metaRepo.GetStepNamesAsync(tenantId, stepIds, ct);
            var scenarioName = scenarioNames.GetValueOrDefault(scenarioId, "—");

            foreach (var s in stats.Where(s => s.Attempts > 0))
            {
                var dropOff = s.UniqueUsers > 0
                    ? Math.Round(1m - (decimal)s.Successful / s.UniqueUsers, 4)
                    : 0m;

                if (dropOff > 0.2m)
                {
                    problemSteps.Add(new ProblemStepSnapshot(
                        scenarioName,
                        stepNames.GetValueOrDefault(s.StepId, "—"),
                        "Unknown",
                        dropOff,
                        s.AvgScoreRatio,
                        s.Attempts));
                }
            }
        }

        var cohorts = await _assignmentRepo.GetCohortRetentionAsync(tenantId, 4, ct);
        var cohortSnapshots = cohorts.Select(c =>
        {
            var ageInDays = today.DayNumber - c.CohortWeekStart.DayNumber;
            var r1 = ageInDays >= 7 && c.CohortSize > 0
                ? Math.Round((decimal)c.ActiveAfterWeek1 / c.CohortSize, 4)
                : (decimal?)null;
            var r4 = ageInDays >= 28 && c.CohortSize > 0
                ? Math.Round((decimal)c.ActiveAfterWeek4 / c.CohortSize, 4)
                : (decimal?)null;
            return new CohortSnapshot(c.CohortWeekStart, c.CohortSize, r1, r4);
        }).ToList();

        return new AnalyticsSnapshot(
            new PeriodInfo(from30, today),
            overall,
            scenarioMetrics,
            problemSteps.OrderByDescending(p => p.FailsRate).Take(5).ToList(),
            cohortSnapshots);
    }
}
