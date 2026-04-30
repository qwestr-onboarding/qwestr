using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence.Models;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Analytics.Application.Queries.Content;

public class GetContentQualityReportQueryHandler
    : IRequestHandler<GetContentQualityReportQuery, ActionResult<ContentQualityReportResponse>>
{
    private readonly IStepCompletionFactRepository _stepRepo;
    private readonly IScenarioMetadataCacheRepository _metaRepo;

    public GetContentQualityReportQueryHandler(
        IStepCompletionFactRepository stepRepo,
        IScenarioMetadataCacheRepository metaRepo)
    {
        _stepRepo = stepRepo;
        _metaRepo = metaRepo;
    }

    public async Task<ActionResult<ContentQualityReportResponse>> Handle(
        GetContentQualityReportQuery request, CancellationToken ct)
    {
        var metadata = await _metaRepo.FindByScenarioIdAsync(request.TenantId, request.ScenarioId, ct);
        var scenarioName = metadata?.ScenarioName ?? "—";

        var stats = await _stepRepo.GetStepStatsAsync(request.TenantId, request.ScenarioId, ct);
        var statsByStep = stats.ToDictionary(s => s.StepId);

        var stepResponses = new List<StepQualityResponse>();

        if (metadata is not null)
        {
            foreach (var stage in metadata.Stages.OrderBy(s => s.Order))
            {
                foreach (var step in stage.Steps.OrderBy(s => s.Order))
                {
                    var stat = statsByStep.GetValueOrDefault(step.StepId);
                    stepResponses.Add(BuildStepResponse(step.StepId, step.StepName, stat));
                }
            }
        }
        else
        {
            foreach (var stat in stats)
            {
                stepResponses.Add(BuildStepResponse(stat.StepId, "—", stat));
            }
        }

        return ActionResult<ContentQualityReportResponse>.Success(
            new ContentQualityReportResponse(request.ScenarioId, scenarioName, stepResponses));
    }

    private static StepQualityResponse BuildStepResponse(Guid stepId, string stepName, StepCompletionStats? stat)
    {
        if (stat is null)
        {
            return new StepQualityResponse(
                stepId, stepName, StepType.Information, 0, 0m, null, 0m, TimeSpan.Zero, QualityFlag.Healthy);
        }

        var dropOff = stat.UniqueUsers > 0
            ? Math.Round(1m - (decimal)stat.Successful / stat.UniqueUsers, 4)
            : 0m;

        var firstAttemptPassRate = stat.Attempts > 0
            ? Math.Round((decimal)stat.FirstAttemptPassCount / stat.Attempts, 4)
            : (decimal?)null;

        var avgAttempts = stat.UniqueUsers > 0
            ? Math.Round((decimal)stat.Attempts / stat.UniqueUsers, 2)
            : 0m;

        var flag = EvaluateFlag(dropOff, firstAttemptPassRate);

        return new StepQualityResponse(
            stepId, stepName, StepType.Information,
            stat.Attempts, dropOff, firstAttemptPassRate, avgAttempts, stat.AvgTimeSpent, flag);
    }

    private static QualityFlag EvaluateFlag(decimal dropOff, decimal? passRate)
    {
        if (dropOff > 0.4m || (passRate.HasValue && passRate.Value < 0.5m))
        {
            return QualityFlag.Critical;
        }

        if (dropOff > 0.2m || (passRate.HasValue && passRate.Value < 0.7m))
        {
            return QualityFlag.NeedsAttention;
        }

        return QualityFlag.Healthy;
    }
}
