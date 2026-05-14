using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm.Options;
using Serilog;

namespace Qwestr.Modules.Analytics.Infrastructure.BackgroundJobs;

public class DailyInsightWarmupJob : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly AnalyticsBackgroundJobsOptions _jobOptions;
    private readonly LlmSharedOptions _llmOptions;
    private readonly ILogger _logger = Log.ForContext<DailyInsightWarmupJob>();
    private DateOnly _lastRunDate = DateOnly.MinValue;

    public DailyInsightWarmupJob(
        IServiceProvider sp,
        IOptions<AnalyticsBackgroundJobsOptions> jobOptions,
        IOptions<LlmSharedOptions> llmOptions)
    {
        _sp = sp;
        _jobOptions = jobOptions.Value;
        _llmOptions = llmOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            if (!_jobOptions.DailyInsightWarmupEnabled)
            {
                continue;
            }

            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);

            if (!TimeOnly.TryParse(_jobOptions.DailyInsightWarmupTimeUtc, out var targetTime))
            {
                targetTime = new TimeOnly(6, 0);
            }

            var currentTime = TimeOnly.FromDateTime(now);
            if (_lastRunDate == today || currentTime < targetTime)
            {
                continue;
            }

            _lastRunDate = today;

            try
            {
                await RunAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DailyInsightWarmupJob failed");
            }
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var snapshotRepo = scope.ServiceProvider.GetRequiredService<IDailyEngagementSnapshotRepository>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var activeTenantIds = await snapshotRepo.GetActiveTenantIdsAsync(today.AddDays(-1), ct);
        _logger.Information("DailyInsightWarmupJob: processing {Count} tenants", activeTenantIds.Count);

        foreach (var tenantId in activeTenantIds)
        {
            try
            {
                await ProcessTenantAsync(scope.ServiceProvider, tenantId, ct);
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DailyInsightWarmupJob: tenant {TenantId} failed", tenantId);
            }
        }
    }

    private async Task ProcessTenantAsync(IServiceProvider sp, Guid tenantId, CancellationToken ct)
    {
        var insightRepo = sp.GetRequiredService<IAnalyticsInsightSetRepository>();
        var snapshotBuilder = sp.GetRequiredService<IAnalyticsSnapshotBuilder>();
        var hasher = sp.GetRequiredService<ISnapshotHasher>();
        var llmFactory = sp.GetRequiredService<ILlmClientFactory>();
        var stepRepo = sp.GetRequiredService<IStepCompletionFactRepository>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var trend = await stepRepo.GetCompletionTrendAsync(tenantId, yesterday, yesterday, ct);
        if (trend.Count == 0 || trend[0].StepsCompleted == 0)
        {
            _logger.Debug("DailyInsightWarmupJob: no activity for tenant {TenantId}, skipping", tenantId);
            return;
        }

        var snapshot = await snapshotBuilder.BuildAsync(tenantId, ct);
        var hash = hasher.ComputeHash(snapshot);

        var cached = await insightRepo.FindByDataHashAsync(tenantId, hash, TimeSpan.FromHours(20), ct);
        if (cached is not null)
        {
            _logger.Debug("DailyInsightWarmupJob: cached insights are fresh for tenant {TenantId}", tenantId);
            return;
        }

        var provider = _llmOptions.DefaultProvider;
        var client = llmFactory.Create(provider);
        var llmResult = await client.GenerateInsightsAsync(snapshot, ct);

        if (llmResult.IsFailure)
        {
            _logger.Warning("DailyInsightWarmupJob: LLM failed for tenant {TenantId}: {Error}",
                tenantId, llmResult.Error);
            return;
        }

        var createResult = AnalyticsInsightSet.Create(
            tenantId,
            DateTime.UtcNow,
            provider,
            InsightTrigger.Scheduled,
            hash,
            llmResult.Value!.Insights,
            llmResult.Value!.Recommendations);

        if (createResult.IsFailure)
        {
            _logger.Warning("DailyInsightWarmupJob: insight creation failed: {Error}", createResult.Error);
            return;
        }

        await insightRepo.AddAsync(createResult.Value!, ct);
        await insightRepo.SaveChangesAsync(ct);

        _logger.Information("DailyInsightWarmupJob: insights saved for tenant {TenantId}", tenantId);
    }
}
