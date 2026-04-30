using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Analytics.Application.Queries.Engagement;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapEngagementEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/engagement/trend", GetEngagementTrendHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Engagement");

        return analytics;
    }

    private static async Task<IResult> GetEngagementTrendHandler(
        int days, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetEngagementTrendQuery(currentUser.TenantId, days), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
