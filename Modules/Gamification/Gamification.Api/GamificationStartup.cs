using Gamification.Api.Endpoints;
using Gamification.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gamification.Api;

public static class GamificationStartup
{
    public static IServiceCollection AddGamificationModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<GamificationDbContext>(o => o.UseNpgsql(config.GetConnectionString("Gamification")));
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapGamificationModule(
        this IEndpointRouteBuilder app)
    {
        app.MapGamificationEndpoints();
        return app;
    }
}