using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.Options;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Insights;

public class GetCooldownStatusQueryHandler : IRequestHandler<GetCooldownStatusQuery, ActionResult<CooldownStatusResponse>>
{
    private readonly IAnalyticsInsightSetRepository _repo;
    private readonly AnalyticsInsightOptions _insightOptions;

    public GetCooldownStatusQueryHandler(
        IAnalyticsInsightSetRepository repo,
        IOptions<AnalyticsInsightOptions> insightOptions)
    {
        _repo = repo;
        _insightOptions = insightOptions.Value;
    }

    public async Task<ActionResult<CooldownStatusResponse>> Handle(
        GetCooldownStatusQuery request, CancellationToken ct)
    {
        var lastAt = await _repo.GetLastManualGeneratedAtAsync(request.TenantId, ct);
        var status = AnalyticsInsightSet.EvaluateCooldown(
            lastAt, DateTime.UtcNow, TimeSpan.FromHours(_insightOptions.InsightCooldownHours));

        var response = new CooldownStatusResponse(
            status.IsAvailable,
            status.Remaining.HasValue ? (int)status.Remaining.Value.TotalSeconds : null,
            status.AvailableAt);

        return ActionResult<CooldownStatusResponse>.Success(response);
    }
}
