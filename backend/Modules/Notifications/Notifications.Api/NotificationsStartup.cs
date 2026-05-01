using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Notifications.Api.Endpoints;
using Qwestr.Modules.Notifications.Application;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Modules.Notifications.Infrastructure;
using Qwestr.Modules.Notifications.Infrastructure.Dispatch;
using Qwestr.Modules.Notifications.Infrastructure.Email;
using Qwestr.Modules.Notifications.Infrastructure.Outbox;
using Qwestr.Modules.Notifications.Infrastructure.Realtime;

namespace Qwestr.Modules.Notifications.Api;

public static class NotificationsStartup
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<EmailOptions>(config.GetSection("Email"));
        services.Configure<AppUrlOptions>(config.GetSection("Email"));
        services.Configure<NotificationDefaultsOptions>(config.GetSection("Notifications:Defaults"));
        services.Configure<NotificationThrottlingOptions>(config.GetSection("Notifications:Throttling"));
        services.Configure<OutboxOptions>(config.GetSection("Notifications:Outbox"));

        services.AddDbContext<NotificationsDbContext>(o =>
            o.UseNpgsql(
                config.GetConnectionString("Notifications"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "notifications"))
             .UseSnakeCaseNamingConvention());

        services.AddScoped<INotificationsDbContext>(sp => sp.GetRequiredService<NotificationsDbContext>());

        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailTemplateRenderer, HtmlEmailTemplateRenderer>();
        services.AddScoped<ISignalRNotificationDispatcher, SignalRNotificationDispatcher>();

        services.AddHostedService<OutboxProcessor>();
        services.AddHostedService<OutboxHousekeepingService>();

        services.AddNotificationsApplication();

        services.AddSignalR();

        services.AddAuthorization(o =>
            o.AddPolicy(NotificationsEndpoints.UserPolicy, p => p.RequireAuthenticatedUser()));

        return services;
    }

    public static IEndpointRouteBuilder MapNotificationsModule(this IEndpointRouteBuilder app)
    {
        app.MapNotificationsEndpoints();
        return app;
    }
}
