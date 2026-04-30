using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Qwestr.Modules.Analytics.Api.Endpoints;

public static partial class AnalyticsEndpoints
{
    internal const string HrPolicy = "AnalyticsHrPolicy";

    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var analytics = app.MapGroup("/api/analytics");

        analytics.MapDashboardEndpoints();
        analytics.MapFunnelEndpoints();
        analytics.MapContentEndpoints();
        analytics.MapEngagementEndpoints();
        analytics.MapRetentionEndpoints();
        analytics.MapInsightsEndpoints();
        analytics.MapExportEndpoints();

        return app;
    }
}
