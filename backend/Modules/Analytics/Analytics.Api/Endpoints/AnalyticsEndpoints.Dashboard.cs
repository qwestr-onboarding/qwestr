using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Analytics.Application.Queries.Dashboard;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/dashboard", GetDashboardHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Dashboard");

        return analytics;
    }

    private static async Task<IResult> GetDashboardHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetTenantDashboardQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
