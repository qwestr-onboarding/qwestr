using Integration.Api.Endpoints;
using Integration.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Api;

public static class IntegrationStartup
{
    public static IServiceCollection AddIntegrationModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IntegrationDbContext>(o => o.UseNpgsql(config.GetConnectionString("Integration")));
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapIntegrationModule(
        this IEndpointRouteBuilder app)
    {
        app.MapIntegrationEndpoints();
        return app;
    }
}