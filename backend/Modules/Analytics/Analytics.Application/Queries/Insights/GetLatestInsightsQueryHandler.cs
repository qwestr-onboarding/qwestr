using MediatR;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Application.Queries.Insights;

public class GetLatestInsightsQueryHandler : IRequestHandler<GetLatestInsightsQuery, ActionResult<AnalyticsInsightSetResponse?>>
{
    private readonly IAnalyticsInsightSetRepository _repo;

    public GetLatestInsightsQueryHandler(IAnalyticsInsightSetRepository repo)
    {
        _repo = repo;
    }

    public async Task<ActionResult<AnalyticsInsightSetResponse?>> Handle(
        GetLatestInsightsQuery request, CancellationToken ct)
    {
        var set = await _repo.GetLatestAsync(request.TenantId, ct);
        if (set is null)
        {
            return ActionResult<AnalyticsInsightSetResponse?>.Success(
                new AnalyticsInsightSetResponse(Guid.Empty, DateTime.MinValue, LlmProvider.YandexGpt, InsightTrigger.Manual, [], []));
        }

        var response = new AnalyticsInsightSetResponse(
            set.Id,
            set.GeneratedAt,
            set.Provider,
            set.Trigger,
            set.Insights.Select(i => new InsightResponse(i.Title, i.Description, i.Severity, i.MetricReference)).ToList(),
            set.Recommendations.Select(r => new RecommendationResponse(r.Title, r.Description, r.Category, r.Priority)).ToList());

        return ActionResult<AnalyticsInsightSetResponse?>.Success(response);
    }
}
