using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence.Models;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Repositories;

public class ScenarioAssignmentFactRepository : IScenarioAssignmentFactRepository
{
    private readonly AnalyticsDbContext _db;

    public ScenarioAssignmentFactRepository(AnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ScenarioAssignmentFact fact, CancellationToken ct)
    {
        await _db.ScenarioAssignmentFacts.AddAsync(fact, ct);
    }

    public async Task<ScenarioAssignmentFact?> FindByAssignmentIdAsync(
        Guid tenantId, Guid assignmentId, CancellationToken ct)
    {
        return await _db.ScenarioAssignmentFacts
            .FirstOrDefaultAsync(f => f.TenantId == tenantId && f.AssignmentId == assignmentId, ct);
    }

    public Task UpdateAsync(ScenarioAssignmentFact fact, CancellationToken ct)
    {
        _db.ScenarioAssignmentFacts.Update(fact);
        return Task.CompletedTask;
    }

    public async Task<ScenarioFunnelData> GetFunnelDataAsync(Guid tenantId, Guid scenarioId, CancellationToken ct)
    {
        var facts = await _db.ScenarioAssignmentFacts
            .Where(f => f.TenantId == tenantId && f.ScenarioId == scenarioId)
            .ToListAsync(ct);

        var assigned = facts.Count;
        var started = facts.Count(f => f.StartedAt is not null);
        var completed = facts.Count(f => f.CurrentStatus == AssignmentStatus.Completed);

        return new ScenarioFunnelData(assigned, started, completed);
    }

    public async Task<IReadOnlyList<CohortRetentionPoint>> GetCohortRetentionAsync(
        Guid tenantId, int weeks, CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddDays(-weeks * 7);

        var facts = await _db.ScenarioAssignmentFacts
            .Where(f => f.TenantId == tenantId && f.AssignedAt >= cutoff)
            .ToListAsync(ct);

        var cohorts = facts
            .GroupBy(f =>
            {
                var daysFromMonday = ((int)f.AssignedAt.DayOfWeek - 1 + 7) % 7;
                return DateOnly.FromDateTime(f.AssignedAt.AddDays(-daysFromMonday).Date);
            })
            .Select(g =>
            {
                var cohortSize = g.Count();
                var week1End = g.Key.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(14);
                var week4End = g.Key.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(35);

                var activeW1 = g.Count(f => f.StartedAt is not null && f.StartedAt <= week1End);
                var activeW4 = g.Count(f => f.StartedAt is not null && f.StartedAt <= week4End);

                return new CohortRetentionPoint(g.Key, cohortSize, activeW1, activeW4);
            })
            .OrderByDescending(c => c.CohortWeekStart)
            .ToList();

        return cohorts;
    }

    public async Task<IReadOnlyList<ScenarioSummaryRow>> GetActiveScenariosSummaryAsync(
        Guid tenantId, CancellationToken ct)
    {
        var raw = await _db.ScenarioAssignmentFacts
            .Where(f => f.TenantId == tenantId)
            .GroupBy(f => f.ScenarioId)
            .Select(g => new
            {
                ScenarioId = g.Key,
                Assigned = g.Count(),
                Started = g.Count(f => f.StartedAt != null),
                Completed = g.Count(f => f.CurrentStatus == AssignmentStatus.Completed)
            })
            .ToListAsync(ct);

        return raw.Select(r => new ScenarioSummaryRow(r.ScenarioId, r.Assigned, r.Started, r.Completed))
            .ToList();
    }

    public async Task<TimeSpan> GetAvgCompletionDurationAsync(Guid tenantId, CancellationToken ct)
    {
        var completed = await _db.ScenarioAssignmentFacts
            .Where(f => f.TenantId == tenantId
                        && f.CurrentStatus == AssignmentStatus.Completed
                        && f.CompletedAt != null)
            .Select(f => new { f.AssignedAt, f.CompletedAt })
            .ToListAsync(ct);

        if (completed.Count == 0)
        {
            return TimeSpan.Zero;
        }

        var avgTicks = completed.Average(f => (f.CompletedAt!.Value - f.AssignedAt).Ticks);
        return TimeSpan.FromTicks((long)avgTicks);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
