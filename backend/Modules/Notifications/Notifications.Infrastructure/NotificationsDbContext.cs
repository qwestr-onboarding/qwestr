using Microsoft.EntityFrameworkCore;
using Qwestr.Shared.Infrastructure;

namespace Qwestr.Modules.Notifications.Infrastructure;

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("notifications");
    }
    
    public override int SaveChanges()
    {
        ChangeTracker.ApplyAuditableEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.ApplyAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }
}