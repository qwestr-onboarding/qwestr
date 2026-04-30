using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Analytics.Application.Queries.Funnel;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapFunnelEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/scenarios/{scenarioId:guid}/funnel", GetFunnelHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Funnel");

        return analytics;
    }

    private static async Task<IResult> GetFunnelHandler(
        Guid scenarioId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetScenarioFunnelQuery(currentUser.TenantId, scenarioId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
