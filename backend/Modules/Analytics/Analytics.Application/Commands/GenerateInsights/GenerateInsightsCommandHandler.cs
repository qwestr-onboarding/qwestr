using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.Options;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Serilog;

namespace Qwestr.Modules.Analytics.Application.Commands.GenerateInsights;

public class GenerateInsightsCommandHandler : IRequestHandler<GenerateInsightsCommand, ActionResult<AnalyticsInsightSetResponse>>
{
    private readonly IAnalyticsInsightSetRepository _insightRepo;
    private readonly IAnalyticsSnapshotBuilder _snapshotBuilder;
    private readonly ISnapshotHasher _hasher;
    private readonly ILlmClientFactory _llmFactory;
    private readonly AnalyticsInsightOptions _insightOptions;
    private readonly ILogger _logger = Log.ForContext<GenerateInsightsCommandHandler>();

    public GenerateInsightsCommandHandler(
        IAnalyticsInsightSetRepository insightRepo,
        IAnalyticsSnapshotBuilder snapshotBuilder,
        ISnapshotHasher hasher,
        ILlmClientFactory llmFactory,
        IOptions<AnalyticsInsightOptions> insightOptions)
    {
        _insightRepo = insightRepo;
        _snapshotBuilder = snapshotBuilder;
        _hasher = hasher;
        _llmFactory = llmFactory;
        _insightOptions = insightOptions.Value;
    }

    public async Task<ActionResult<AnalyticsInsightSetResponse>> Handle(
        GenerateInsightsCommand request, CancellationToken ct)
    {
        var lastAt = await _insightRepo.GetLastManualGeneratedAtAsync(request.TenantId, ct);
        var cooldown = AnalyticsInsightSet.EvaluateCooldown(
            lastAt, DateTime.UtcNow, TimeSpan.FromHours(_insightOptions.InsightCooldownHours));

        if (!cooldown.IsAvailable)
        {
            return ActionResult<AnalyticsInsightSetResponse>.Failure("analytics.insight.cooldown_active");
        }

        var snapshot = await _snapshotBuilder.BuildAsync(request.TenantId, ct);
        var hash = _hasher.ComputeHash(snapshot);

        var cached = await _insightRepo.FindByDataHashAsync(
            request.TenantId, hash, TimeSpan.FromHours(1), ct);

        if (cached is not null)
        {
            // return ActionResult<AnalyticsInsightSetResponse>.Success(MapToResponse(cached));
        }

        var client = _llmFactory.Create(request.Provider);

        _logger.Information("Generating insights via {Provider} for tenant {TenantId}", request.Provider, request.TenantId);
        var started = DateTime.UtcNow;

        var llmResult = await client.GenerateInsightsAsync(snapshot, ct);

        if (llmResult.IsFailure)
        {
            _logger.Warning("LLM call failed for tenant {TenantId}: {Error}", request.TenantId, llmResult.Error);
            return ActionResult<AnalyticsInsightSetResponse>.Failure(llmResult.Error!);
        }

        var elapsed = DateTime.UtcNow - started;
        _logger.Information("LLM call succeeded in {ElapsedMs}ms for tenant {TenantId}",
            elapsed.TotalMilliseconds, request.TenantId);

        var llmResponse = llmResult.Value!;
        var createResult = AnalyticsInsightSet.Create(
            request.TenantId,
            DateTime.UtcNow,
            request.Provider,
            InsightTrigger.Manual,
            hash,
            llmResponse.Insights,
            llmResponse.Recommendations);

        if (createResult.IsFailure)
        {
            _logger.Error("Failed to create AnalyticsInsightSet for tenant {TenantId}: {Error}",
                request.TenantId, createResult.Error);
            return ActionResult<AnalyticsInsightSetResponse>.Failure(createResult.Error!);
        }

        await _insightRepo.AddAsync(createResult.Value!, ct);
        await _insightRepo.SaveChangesAsync(ct);

        return ActionResult<AnalyticsInsightSetResponse>.Success(MapToResponse(createResult.Value!));
    }

    private static AnalyticsInsightSetResponse MapToResponse(AnalyticsInsightSet set)
    {
        return new AnalyticsInsightSetResponse(
            set.Id,
            set.GeneratedAt,
            set.Provider,
            set.Trigger,
            set.Insights.Select(i => new InsightResponse(i.Title, i.Description, i.Severity, i.MetricReference)).ToList(),
            set.Recommendations.Select(r => new RecommendationResponse(r.Title, r.Description, r.Category, r.Priority)).ToList());
    }
}
