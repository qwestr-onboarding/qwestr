using Microsoft.EntityFrameworkCore;
using Qwestr.Shared.Infrastructure;

namespace Qwestr.Modules.Gamification.Infrastructure;

public class GamificationDbContext : DbContext
{
    public GamificationDbContext(DbContextOptions<GamificationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gamification");
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