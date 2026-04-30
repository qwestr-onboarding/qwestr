using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Analytics.Api.Endpoints;
using Qwestr.Modules.Analytics.Application;
using Qwestr.Modules.Analytics.Infrastructure;

namespace Qwestr.Modules.Analytics.Api;

public static class AnalyticsStartup
{
    public static IServiceCollection AddAnalyticsModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddAnalyticsApplication();
        services.AddAnalyticsInfrastructure(config);

        services.AddAuthorization(o =>
            o.AddPolicy(AnalyticsEndpoints.HrPolicy, p => p.RequireRole("Manager", "Admin", "Owner")));

        return services;
    }

    public static IEndpointRouteBuilder MapAnalyticsModule(this IEndpointRouteBuilder app)
    {
        app.MapAnalyticsEndpoints();
        return app;
    }
}
