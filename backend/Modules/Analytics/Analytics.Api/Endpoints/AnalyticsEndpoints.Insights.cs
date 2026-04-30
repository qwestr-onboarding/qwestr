using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Analytics.Application.Commands.GenerateInsights;
using Qwestr.Modules.Analytics.Application.Queries.Insights;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Requests;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapInsightsEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/insights/latest", GetLatestInsightsHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Insights");

        analytics.MapPost("/insights/generate", GenerateInsightsHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Insights");

        analytics.MapGet("/insights/cooldown", GetCooldownHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Insights");

        return analytics;
    }

    private static async Task<IResult> GetLatestInsightsHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetLatestInsightsQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GenerateInsightsHandler(
        GenerateInsightsRequest body,
        ICurrentUserService currentUser,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GenerateInsightsCommand(currentUser.TenantId, body.Provider), ct);

        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        if (result.Error == "analytics.insight.cooldown_active")
        {
            var cooldown = await sender.Send(new GetCooldownStatusQuery(currentUser.TenantId), ct);
            var availableAt = cooldown.Value?.AvailableAt?.ToString("O") ?? string.Empty;

            return Results.Json(
                new { error = "cooldown_active", availableAt },
                statusCode: 429,
                contentType: "application/json");
        }

        return Results.BadRequest(new ErrorResponse(result.Error));
    }

    private static async Task<IResult> GetCooldownHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetCooldownStatusQuery(currentUser.TenantId), ct);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
