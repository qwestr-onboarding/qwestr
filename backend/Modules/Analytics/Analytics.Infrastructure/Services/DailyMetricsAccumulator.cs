using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Infrastructure.Services;

public class DailyMetricsAccumulator : IDailyMetricsAccumulator
{
    private readonly AnalyticsDbContext _db;

    public DailyMetricsAccumulator(AnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task IncrementAsync(Guid tenantId, DailyMetricKind kind, int delta, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var snapshot = await _db.DailyEngagementSnapshots
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Date == today, ct);

        if (snapshot is null)
        {
            var createResult = DailyEngagementSnapshot.Create(
                tenantId, today, 0, 0, 0, 0, 0, 0, DateTime.UtcNow);

            if (createResult.IsFailure)
            {
                return;
            }

            snapshot = createResult.Value!;
            await _db.DailyEngagementSnapshots.AddAsync(snapshot, ct);
            await _db.SaveChangesAsync(ct);
        }

        var (activeUsers, steps, scenariosStarted, scenariosCompleted, badges, points) = (
            snapshot.ActiveUsers,
            snapshot.StepsCompleted,
            snapshot.ScenariosStarted,
            snapshot.ScenariosCompleted,
            snapshot.BadgesEarned,
            snapshot.TotalPointsGranted);

        switch (kind)
        {
            case DailyMetricKind.StepsCompleted: steps += delta; break;
            case DailyMetricKind.ScenariosStarted: scenariosStarted += delta; break;
            case DailyMetricKind.ScenariosCompleted: scenariosCompleted += delta; break;
            case DailyMetricKind.BadgesEarned: badges += delta; break;
            case DailyMetricKind.TotalPointsGranted: points += delta; break;
            case DailyMetricKind.ActiveUsers: activeUsers += delta; break;
        }

        snapshot.Recalculate(activeUsers, steps, scenariosStarted, scenariosCompleted, badges, points, DateTime.UtcNow);
        _db.DailyEngagementSnapshots.Update(snapshot);
        await _db.SaveChangesAsync(ct);
    }
}
