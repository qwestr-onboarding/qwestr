using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class PointsTransactionConfiguration : IEntityTypeConfiguration<PointsTransaction>
{
    public void Configure(EntityTypeBuilder<PointsTransaction> builder)
    {
        builder.ToTable("points_transactions", "gamification");

        builder.Property(t => t.SourceType).HasConversion<int>();
        builder.Property(t => t.Reason).HasMaxLength(500);

        builder.HasIndex(t => new { t.ProfileId, t.SourceType, t.SourceId })
            .IsUnique()
            .HasDatabaseName("ix_points_transactions_idempotency");

        builder.HasIndex(t => new { t.TenantId, t.ProfileId, t.OccurredAt });
    }
}
