using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Infrastructure;

namespace Qwestr.Modules.Gamification.Infrastructure;

public class GamificationDbContext : DbContext
{
    public GamificationDbContext(DbContextOptions<GamificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlayerProfile> PlayerProfiles => Set<PlayerProfile>();
    public DbSet<PointsTransaction> PointsTransactions => Set<PointsTransaction>();
    public DbSet<CoinTransaction> CoinTransactions => Set<CoinTransaction>();
    public DbSet<LevelTier> LevelTiers => Set<LevelTier>();
    public DbSet<Award> Awards => Set<Award>();
    public DbSet<AchievementProgress> AchievementProgresses => Set<AchievementProgress>();
    public DbSet<EarnedAward> EarnedAwards => Set<EarnedAward>();
    public DbSet<ShopItem> ShopItems => Set<ShopItem>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("gamification");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
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