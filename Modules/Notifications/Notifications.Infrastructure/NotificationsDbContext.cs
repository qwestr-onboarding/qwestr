using Microsoft.EntityFrameworkCore;

namespace Notifications.Infrastructure;

public class NotificationsDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("notifications");
    }
}