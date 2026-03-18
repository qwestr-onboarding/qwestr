using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class StageConfiguration : IEntityTypeConfiguration<Stage>
{
    public void Configure(EntityTypeBuilder<Stage> builder)
    {
        builder.Property(s => s.Title).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(1000);

        builder.HasIndex(s => s.ScenarioId);

        builder.HasOne(s => s.Scenario)
            .WithMany(sc => sc.Stages)
            .HasForeignKey(s => s.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
