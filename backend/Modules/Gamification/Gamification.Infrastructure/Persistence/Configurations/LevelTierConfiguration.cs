using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class LevelTierConfiguration : IEntityTypeConfiguration<LevelTier>
{
    public void Configure(EntityTypeBuilder<LevelTier> builder)
    {
        builder.ToTable("level_tiers", "gamification");

        builder.Property(t => t.Title).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Color).HasMaxLength(20).IsRequired();
        builder.Property(t => t.FrameUrl).HasMaxLength(500);

        builder.HasIndex(t => new { t.TenantId, t.Order });
    }
}
