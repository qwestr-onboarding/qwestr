using Microsoft.EntityFrameworkCore;
using Qwestr.Shared.Infrastructure;

namespace Qwestr.Modules.Analytics.Infrastructure;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("analytics");
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