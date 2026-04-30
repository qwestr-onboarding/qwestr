using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Serilog;

namespace Qwestr.Modules.Analytics.Infrastructure.BackgroundJobs;

public class DailySnapshotJob : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly AnalyticsBackgroundJobsOptions _options;
    private readonly ILogger _logger = Log.ForContext<DailySnapshotJob>();
    private DateOnly _lastRunDate = DateOnly.MinValue;

    public DailySnapshotJob(IServiceProvider sp, IOptions<AnalyticsBackgroundJobsOptions> options)
    {
        _sp = sp;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            if (!_options.DailySnapshotEnabled)
            {
                continue;
            }

            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);

            if (!TimeOnly.TryParse(_options.DailySnapshotTimeUtc, out var targetTime))
            {
                targetTime = new TimeOnly(2, 0);
            }

            var currentTime = TimeOnly.FromDateTime(now);
            if (_lastRunDate == today || currentTime < targetTime)
            {
                continue;
            }

            _lastRunDate = today;
            var yesterday = today.AddDays(-1);

            try
            {
                await RunAsync(yesterday, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DailySnapshotJob failed for date {Date}", yesterday);
            }
        }
    }

    private async Task RunAsync(DateOnly date, CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var snapshotRepo = scope.ServiceProvider.GetRequiredService<IDailyEngagementSnapshotRepository>();

        var activeTenantIds = await snapshotRepo.GetActiveTenantIdsAsync(date.AddDays(-30), ct);
        _logger.Information("DailySnapshotJob: processing {Count} tenants for {Date}", activeTenantIds.Count, date);

        foreach (var tenantId in activeTenantIds)
        {
            try
            {
                await ProcessTenantAsync(scope.ServiceProvider, tenantId, date, ct);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DailySnapshotJob: tenant {TenantId} failed", tenantId);
            }
        }
    }

    private static async Task ProcessTenantAsync(
        IServiceProvider sp, Guid tenantId, DateOnly date, CancellationToken ct)
    {
        var snapshotRepo = sp.GetRequiredService<IDailyEngagementSnapshotRepository>();
        var stepRepo = sp.GetRequiredService<IStepCompletionFactRepository>();

        var trend = await stepRepo.GetCompletionTrendAsync(tenantId, date, date, ct);
        var stepsCompleted = trend.FirstOrDefault()?.StepsCompleted ?? 0;
        var activeUsers = await snapshotRepo.CountDistinctActiveUsersAsync(tenantId, date, ct);

        var existing = await snapshotRepo.FindByDateAsync(tenantId, date, ct);

        if (existing is null)
        {
            var createResult = DailyEngagementSnapshot.Create(
                tenantId, date, activeUsers, stepsCompleted, 0, 0, 0, 0, DateTime.UtcNow);

            if (createResult.IsSuccess)
            {
                await snapshotRepo.AddAsync(createResult.Value!, ct);
            }
        }
        else
        {
            existing.Recalculate(
                activeUsers,
                stepsCompleted,
                existing.ScenariosStarted,
                existing.ScenariosCompleted,
                existing.BadgesEarned,
                existing.TotalPointsGranted,
                DateTime.UtcNow);
            await snapshotRepo.UpdateAsync(existing, ct);
        }

        await snapshotRepo.SaveChangesAsync(ct);
    }
}
