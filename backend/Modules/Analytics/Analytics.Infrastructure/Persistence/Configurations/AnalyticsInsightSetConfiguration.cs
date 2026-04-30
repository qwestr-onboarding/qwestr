using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Configurations;

public class AnalyticsInsightSetConfiguration : IEntityTypeConfiguration<AnalyticsInsightSet>
{
    public void Configure(EntityTypeBuilder<AnalyticsInsightSet> builder)
    {
        builder.ToTable("analytics_insight_sets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.GeneratedAt).IsRequired();
        builder.Property(x => x.Provider).IsRequired();
        builder.Property(x => x.Trigger).IsRequired();
        builder.Property(x => x.DataSnapshotHash).HasMaxLength(128).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.GeneratedAt });
        builder.HasIndex(x => new { x.TenantId, x.Trigger, x.GeneratedAt });
        builder.HasIndex(x => new { x.TenantId, x.DataSnapshotHash });

        builder.HasMany(x => x.Insights)
            .WithOne()
            .HasForeignKey("InsightSetId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Recommendations)
            .WithOne()
            .HasForeignKey("InsightSetId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class InsightConfiguration : IEntityTypeConfiguration<Insight>
{
    public void Configure(EntityTypeBuilder<Insight> builder)
    {
        builder.ToTable("insights");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Severity).IsRequired();
        builder.Property(x => x.MetricReference).HasMaxLength(100).IsRequired();
    }
}

public class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
{
    public void Configure(EntityTypeBuilder<Recommendation> builder)
    {
        builder.ToTable("recommendations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Category).IsRequired();
        builder.Property(x => x.Priority).IsRequired();
    }
}
