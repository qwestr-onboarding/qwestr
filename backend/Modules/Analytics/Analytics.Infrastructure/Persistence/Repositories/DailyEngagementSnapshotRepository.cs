using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Repositories;

public class DailyEngagementSnapshotRepository : IDailyEngagementSnapshotRepository
{
    private readonly AnalyticsDbContext _db;

    public DailyEngagementSnapshotRepository(AnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task<DailyEngagementSnapshot?> FindByDateAsync(Guid tenantId, DateOnly date, CancellationToken ct)
    {
        return await _db.DailyEngagementSnapshots
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Date == date, ct);
    }

    public async Task AddAsync(DailyEngagementSnapshot snapshot, CancellationToken ct)
    {
        await _db.DailyEngagementSnapshots.AddAsync(snapshot, ct);
    }

    public Task UpdateAsync(DailyEngagementSnapshot snapshot, CancellationToken ct)
    {
        _db.DailyEngagementSnapshots.Update(snapshot);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<DailyEngagementSnapshot>> GetRangeAsync(
        Guid tenantId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        return await _db.DailyEngagementSnapshots
            .Where(s => s.TenantId == tenantId && s.Date >= from && s.Date <= to)
            .OrderBy(s => s.Date)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Guid>> GetActiveTenantIdsAsync(DateOnly since, CancellationToken ct)
    {
        return await _db.DailyEngagementSnapshots
            .Where(s => s.Date >= since)
            .Select(s => s.TenantId)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<int> CountDistinctActiveUsersAsync(Guid tenantId, DateOnly date, CancellationToken ct)
    {
        var fromDt = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toDt = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        return await _db.StepCompletionFacts
            .Where(f => f.TenantId == tenantId && f.CompletedAt >= fromDt && f.CompletedAt <= toDt)
            .Select(f => f.UserId)
            .Distinct()
            .CountAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
