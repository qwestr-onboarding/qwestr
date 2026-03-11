using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Notifications.Api.Endpoints;
using Qwestr.Modules.Notifications.Infrastructure;

namespace Qwestr.Modules.Notifications.Api;

public static class NotificationsStartup
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<NotificationsDbContext>(o => o.UseNpgsql(config.GetConnectionString("Notifications")).UseSnakeCaseNamingConvention());
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapNotificationsModule(
        this IEndpointRouteBuilder app)
    {
        app.MapNotificationsEndpoints();
        return app;
    }
}