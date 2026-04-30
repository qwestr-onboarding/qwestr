using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Analytics.Application.Queries.Content;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/scenarios/{scenarioId:guid}/content-quality", GetContentQualityHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Content");

        return analytics;
    }

    private static async Task<IResult> GetContentQualityHandler(
        Guid scenarioId, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetContentQualityReportQuery(currentUser.TenantId, scenarioId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
