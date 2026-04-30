using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Repositories;

public class AnalyticsInsightSetRepository : IAnalyticsInsightSetRepository
{
    private readonly AnalyticsDbContext _db;

    public AnalyticsInsightSetRepository(AnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AnalyticsInsightSet set, CancellationToken ct)
    {
        await _db.AnalyticsInsightSets.AddAsync(set, ct);
    }

    public async Task<AnalyticsInsightSet?> GetLatestAsync(Guid tenantId, CancellationToken ct)
    {
        return await _db.AnalyticsInsightSets
            .Include(s => s.Insights)
            .Include(s => s.Recommendations)
            .Where(s => s.TenantId == tenantId)
            .OrderByDescending(s => s.GeneratedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<DateTime?> GetLastManualGeneratedAtAsync(Guid tenantId, CancellationToken ct)
    {
        return await _db.AnalyticsInsightSets
            .Where(s => s.TenantId == tenantId && s.Trigger == InsightTrigger.Manual)
            .OrderByDescending(s => s.GeneratedAt)
            .Select(s => (DateTime?)s.GeneratedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<AnalyticsInsightSet?> FindByDataHashAsync(
        Guid tenantId, string snapshotHash, TimeSpan freshness, CancellationToken ct)
    {
        var minDate = DateTime.UtcNow - freshness;

        return await _db.AnalyticsInsightSets
            .Include(s => s.Insights)
            .Include(s => s.Recommendations)
            .Where(s => s.TenantId == tenantId
                        && s.DataSnapshotHash == snapshotHash
                        && s.GeneratedAt >= minDate)
            .OrderByDescending(s => s.GeneratedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
