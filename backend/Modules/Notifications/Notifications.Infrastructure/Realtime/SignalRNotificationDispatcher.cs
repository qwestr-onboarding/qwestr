using Microsoft.AspNetCore.SignalR;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;

namespace Qwestr.Modules.Notifications.Infrastructure.Realtime;

public sealed class SignalRNotificationDispatcher : ISignalRNotificationDispatcher
{
    private readonly IHubContext<NotificationsHub> _hubContext;

    public SignalRNotificationDispatcher(IHubContext<NotificationsHub> hubContext)
        => _hubContext = hubContext;

    public async Task PushAsync(Guid userId, NotificationResponse notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients
            .Group($"user:{userId}")
            .SendAsync("NewNotification", notification, cancellationToken);
    }
}
