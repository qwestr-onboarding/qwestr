using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class EarnedAwardConfiguration : IEntityTypeConfiguration<EarnedAward>
{
    public void Configure(EntityTypeBuilder<EarnedAward> builder)
    {
        builder.ToTable("earned_awards", "gamification");

        builder.HasIndex(e => new { e.ProfileId, e.AwardId }).IsUnique();
        builder.HasIndex(e => e.ProfileId);
    }
}
