using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Configurations;

public class DailyEngagementSnapshotConfiguration : IEntityTypeConfiguration<DailyEngagementSnapshot>
{
    public void Configure(EntityTypeBuilder<DailyEngagementSnapshot> builder)
    {
        builder.ToTable("daily_engagement_snapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.ActiveUsers).IsRequired();
        builder.Property(x => x.StepsCompleted).IsRequired();
        builder.Property(x => x.ScenariosStarted).IsRequired();
        builder.Property(x => x.ScenariosCompleted).IsRequired();
        builder.Property(x => x.BadgesEarned).IsRequired();
        builder.Property(x => x.TotalPointsGranted).IsRequired();
        builder.Property(x => x.CalculatedAt).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.Date }).IsUnique();
    }
}
