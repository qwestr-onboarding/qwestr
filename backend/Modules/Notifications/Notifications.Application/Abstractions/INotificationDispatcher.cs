using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Abstractions;

public interface INotificationDispatcher
{
    Task<ActionResult> DispatchAsync(NotificationContext context, CancellationToken cancellationToken = default);
}
