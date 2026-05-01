using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Qwestr.Modules.Notifications.Infrastructure.Realtime;

namespace Qwestr.Modules.Notifications.Api.Endpoints;

public static partial class NotificationsEndpoints
{
    internal const string UserPolicy = "NotificationsUserPolicy";

    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications")
            .RequireAuthorization(UserPolicy);

        group.MapNotificationsListEndpoints();
        group.MapPreferencesEndpoints();

        app.MapHub<NotificationsHub>("/hubs/notifications");

        return app;
    }
}
