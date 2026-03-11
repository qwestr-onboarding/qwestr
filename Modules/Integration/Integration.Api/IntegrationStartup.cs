using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Integration.Api.Endpoints;
using Qwestr.Modules.Integration.Infrastructure;

namespace Qwestr.Modules.Integration.Api;

public static class IntegrationStartup
{
    public static IServiceCollection AddIntegrationModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IntegrationDbContext>(o => o.UseNpgsql(config.GetConnectionString("Integration")).UseSnakeCaseNamingConvention());
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapIntegrationModule(
        this IEndpointRouteBuilder app)
    {
        app.MapIntegrationEndpoints();
        return app;
    }
}