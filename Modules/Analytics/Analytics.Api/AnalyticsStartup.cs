using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Analytics.Api.Endpoints;
using Qwestr.Modules.Analytics.Infrastructure;

namespace Qwestr.Modules.Analytics.Api;

public static class AnalyticsStartup
{
    public static IServiceCollection AddAnalyticsModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AnalyticsDbContext>(o => o.UseNpgsql(config.GetConnectionString("Analytics")).UseSnakeCaseNamingConvention());
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapAnalyticsModule(
        this IEndpointRouteBuilder app)
    {
        app.MapAnalyticsEndpoints();
        return app;
    }
}