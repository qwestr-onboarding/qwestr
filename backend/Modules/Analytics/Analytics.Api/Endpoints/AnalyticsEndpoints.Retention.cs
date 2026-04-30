using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Analytics.Application.Queries.Retention;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapRetentionEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/cohorts/retention", GetCohortRetentionHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Retention");

        return analytics;
    }

    private static async Task<IResult> GetCohortRetentionHandler(
        int weeks, ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetCohortRetentionQuery(currentUser.TenantId, weeks), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
