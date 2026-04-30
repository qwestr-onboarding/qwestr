using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;
using Qwestr.Shared.Infrastructure;

namespace Qwestr.Modules.Analytics.Infrastructure;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<StepCompletionFact> StepCompletionFacts { get; set; } = null!;
    public DbSet<ScenarioAssignmentFact> ScenarioAssignmentFacts { get; set; } = null!;
    public DbSet<DailyEngagementSnapshot> DailyEngagementSnapshots { get; set; } = null!;
    public DbSet<ScenarioMetadataCache> ScenarioMetadataCaches { get; set; } = null!;
    public DbSet<StageMetadataItem> StageMetadataItems { get; set; } = null!;
    public DbSet<StepMetadataItem> StepMetadataItems { get; set; } = null!;
    public DbSet<AnalyticsInsightSet> AnalyticsInsightSets { get; set; } = null!;
    public DbSet<Insight> Insights { get; set; } = null!;
    public DbSet<Recommendation> Recommendations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("analytics");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnalyticsDbContext).Assembly);
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