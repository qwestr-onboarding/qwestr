using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;

namespace Qwestr.Modules.Notifications.Application.Abstractions;

public interface ISignalRNotificationDispatcher
{
    Task PushAsync(Guid userId, NotificationResponse notification, CancellationToken cancellationToken = default);
}
