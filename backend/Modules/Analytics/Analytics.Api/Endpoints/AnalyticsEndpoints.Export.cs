using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Infrastructure;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    private static IEndpointRouteBuilder MapExportEndpoints(this IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/export", ExportCsvHandler)
            .RequireAuthorization(HrPolicy)
            .WithTags("Analytics.Export");

        return analytics;
    }

    private static async Task<IResult> ExportCsvHandler(
        string? format,
        DateTime? from,
        DateTime? to,
        ICurrentUserService currentUser,
        AnalyticsDbContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var tenantId = currentUser.TenantId;
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow;

        httpContext.Response.ContentType = "text/csv";
        httpContext.Response.Headers.Append("Content-Disposition",
            $"attachment; filename=\"analytics_{DateOnly.FromDateTime(DateTime.UtcNow)}.csv\"");

        await using var writer = new StreamWriter(httpContext.Response.Body, leaveOpen: true);
        await writer.WriteLineAsync(
            "step_id,scenario_id,user_id,completed_at,time_spent_seconds,was_successful,attempt_number,score,max_score");

        var query = db.StepCompletionFacts
            .Where(f => f.TenantId == tenantId
                        && f.CompletedAt >= fromDate
                        && f.CompletedAt <= toDate)
            .OrderBy(f => f.CompletedAt)
            .AsAsyncEnumerable();

        await foreach (var fact in query.WithCancellation(ct))
        {
            await writer.WriteLineAsync(
                $"{fact.StepId},{fact.ScenarioId},{fact.UserId}," +
                $"{fact.CompletedAt:O},{(long)fact.TimeSpent.TotalSeconds}," +
                $"{fact.WasSuccessful},{fact.AttemptNumber}," +
                $"{fact.Score?.ToString() ?? ""},{fact.MaxScore?.ToString() ?? ""}");
        }

        await writer.FlushAsync(ct);
        return Results.Empty;
    }
}
