using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class AchievementProgressConfiguration : IEntityTypeConfiguration<AchievementProgress>
{
    public void Configure(EntityTypeBuilder<AchievementProgress> builder)
    {
        builder.ToTable("achievement_progresses", "gamification");

        builder.HasIndex(p => new { p.ProfileId, p.AwardId }).IsUnique();
        builder.HasIndex(p => p.ProfileId);
    }
}
