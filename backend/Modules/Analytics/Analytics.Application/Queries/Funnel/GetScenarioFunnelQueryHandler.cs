using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Funnel;

public class GetScenarioFunnelQueryHandler : IRequestHandler<GetScenarioFunnelQuery, ActionResult<ScenarioFunnelResponse>>
{
    private readonly IScenarioAssignmentFactRepository _assignmentRepo;
    private readonly IStepCompletionFactRepository _stepRepo;
    private readonly IScenarioMetadataCacheRepository _metaRepo;

    public GetScenarioFunnelQueryHandler(
        IScenarioAssignmentFactRepository assignmentRepo,
        IStepCompletionFactRepository stepRepo,
        IScenarioMetadataCacheRepository metaRepo)
    {
        _assignmentRepo = assignmentRepo;
        _stepRepo = stepRepo;
        _metaRepo = metaRepo;
    }

    public async Task<ActionResult<ScenarioFunnelResponse>> Handle(
        GetScenarioFunnelQuery request, CancellationToken ct)
    {
        var funnel = await _assignmentRepo.GetFunnelDataAsync(request.TenantId, request.ScenarioId, ct);
        var metadata = await _metaRepo.FindByScenarioIdAsync(request.TenantId, request.ScenarioId, ct);

        var scenarioName = metadata?.ScenarioName ?? "—";
        var completionRate = funnel.Assigned > 0
            ? Math.Round((decimal)funnel.Completed / funnel.Assigned, 4)
            : 0m;

        var stages = new List<StageFunnelResponse>();

        if (metadata is not null)
        {
            var stepStats = await _stepRepo.GetStepStatsAsync(request.TenantId, request.ScenarioId, ct);
            var statsByStep = stepStats.ToDictionary(s => s.StepId);

            foreach (var stage in metadata.Stages.OrderBy(s => s.Order))
            {
                var stepResponses = new List<StepFunnelResponse>();
                var stageReached = 0;
                var stageTotalTime = TimeSpan.Zero;
                var orderedSteps = stage.Steps.OrderBy(s => s.Order).ToList();

                foreach (var step in orderedSteps)
                {
                    var stat = statsByStep.GetValueOrDefault(step.StepId);
                    var reached = stat?.UniqueUsers ?? 0;
                    var completed = stat?.Successful ?? 0;
                    var dropOff = reached > 0 ? Math.Round(1m - (decimal)completed / reached, 4) : 0m;
                    var avgTime = stat?.AvgTimeSpent ?? TimeSpan.Zero;

                    stageReached = Math.Max(stageReached, reached);
                    stageTotalTime += avgTime;

                    stepResponses.Add(new StepFunnelResponse(step.StepId, step.StepName, reached, completed, dropOff, avgTime));
                }

                // stageCompleted = UniqueUsers последнего шага: сколько пользователей дошло до конца этапа
                var lastStepStat = orderedSteps.Count > 0
                    ? statsByStep.GetValueOrDefault(orderedSteps[^1].StepId)
                    : null;
                var stageCompleted = lastStepStat?.UniqueUsers ?? 0;

                var stageAvgTime = stepResponses.Count > 0
                    ? TimeSpan.FromTicks(stageTotalTime.Ticks / stepResponses.Count)
                    : TimeSpan.Zero;

                stages.Add(new StageFunnelResponse(
                    stage.StageId, stage.StageName, stageReached, stageCompleted, stageAvgTime, stepResponses));
            }
        }

        var response = new ScenarioFunnelResponse(
            request.ScenarioId, scenarioName,
            funnel.Assigned, funnel.Started, funnel.Completed,
            completionRate, stages);

        return ActionResult<ScenarioFunnelResponse>.Success(response);
    }
}
