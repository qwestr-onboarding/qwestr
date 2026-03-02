using Identity.Api.Endpoints;
using Identity.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api;

public static class IdentityStartup
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IdentityDbContext>(o => o.UseNpgsql(config.GetConnectionString("Identity")));
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapIdentityModule(
        this IEndpointRouteBuilder app)
    {
        app.MapIdentityEndpoints();
        return app;
    }
}