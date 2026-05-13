using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Domain.Entities;

public class NotificationDispatchLog : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public DateTimeOffset DispatchedAt { get; private set; }

    private NotificationDispatchLog() { }

    public static NotificationDispatchLog Create(
        Guid tenantId,
        Guid userId,
        NotificationType type,
        NotificationChannel channel,
        DateTimeOffset dispatchedAt)
    {
        return new NotificationDispatchLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Type = type,
            Channel = channel,
            DispatchedAt = dispatchedAt
        };
    }
}