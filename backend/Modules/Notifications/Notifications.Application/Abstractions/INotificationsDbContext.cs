using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Notifications.Domain.Entities;

namespace Qwestr.Modules.Notifications.Application.Abstractions;

public interface INotificationsDbContext
{
    DbSet<Notification> Notifications { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<NotificationDispatchLog> NotificationDispatchLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
