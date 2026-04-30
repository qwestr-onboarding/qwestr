using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence.Models;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Repositories;

public class StepCompletionFactRepository : IStepCompletionFactRepository
{
    private readonly AnalyticsDbContext _db;

    public StepCompletionFactRepository(AnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(StepCompletionFact fact, CancellationToken ct)
    {
        await _db.StepCompletionFacts.AddAsync(fact, ct);
    }

    public async Task<bool> ExistsAsync(Guid tenantId, Guid stepId, Guid userId, int attemptNumber, CancellationToken ct)
    {
        return await _db.StepCompletionFacts.AnyAsync(
            f => f.TenantId == tenantId
                 && f.StepId == stepId
                 && f.UserId == userId
                 && f.AttemptNumber == attemptNumber, ct);
    }

    public async Task<IReadOnlyList<StepCompletionStats>> GetStepStatsAsync(
        Guid tenantId, Guid scenarioId, CancellationToken ct)
    {
        var facts = await _db.StepCompletionFacts
            .Where(f => f.TenantId == tenantId && f.ScenarioId == scenarioId)
            .ToListAsync(ct);

        return facts
            .GroupBy(f => f.StepId)
            .Select(g =>
            {
                var scoredAttempts = g.Where(f => f.Score != null && f.MaxScore != null && f.MaxScore > 0).ToList();
                var avgScoreRatio = scoredAttempts.Count > 0
                    ? scoredAttempts.Average(f => (decimal)f.Score! / f.MaxScore!.Value)
                    : (decimal?)null;

                return new StepCompletionStats(
                    g.Key,
                    g.Count(),
                    g.Count(f => f.WasSuccessful),
                    TimeSpan.FromTicks((long)g.Average(f => (double)f.TimeSpent.Ticks)),
                    avgScoreRatio,
                    g.Count(f => f.AttemptNumber == 1 && f.WasSuccessful),
                    g.Select(f => f.UserId).Distinct().Count());
            })
            .ToList();
    }

    public async Task<IReadOnlyList<DailyCompletionPoint>> GetCompletionTrendAsync(
        Guid tenantId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var fromDt = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toDt = to.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var raw = await _db.StepCompletionFacts
            .Where(f => f.TenantId == tenantId
                        && f.CompletedAt >= fromDt
                        && f.CompletedAt <= toDt
                        && f.WasSuccessful)
            .GroupBy(f => f.CompletedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return raw
            .Select(r => new DailyCompletionPoint(DateOnly.FromDateTime(r.Date), r.Count))
            .OrderBy(p => p.Date)
            .ToList();
    }

    public async Task<int> CountAtRiskEmployeesAsync(Guid tenantId, TimeSpan inactivityThreshold, CancellationToken ct)
    {
        var threshold = DateTime.UtcNow - inactivityThreshold;

        var inProgressUserIds = await _db.ScenarioAssignmentFacts
            .Where(f => f.TenantId == tenantId
                        && f.CurrentStatus == Shared.Kernel.Enums.Onboarding.AssignmentStatus.InProgress)
            .Select(f => f.UserId)
            .Distinct()
            .ToListAsync(ct);

        if (inProgressUserIds.Count == 0)
        {
            return 0;
        }

        var recentActiveUserIds = await _db.StepCompletionFacts
            .Where(f => f.TenantId == tenantId && f.CompletedAt >= threshold)
            .Select(f => f.UserId)
            .Distinct()
            .ToListAsync(ct);

        return inProgressUserIds.Except(recentActiveUserIds).Count();
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
