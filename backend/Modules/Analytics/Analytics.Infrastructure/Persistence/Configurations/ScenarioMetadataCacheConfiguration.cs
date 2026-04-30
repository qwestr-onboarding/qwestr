using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Analytics.Domain.Entities.Snapshots;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Configurations;

public class ScenarioMetadataCacheConfiguration : IEntityTypeConfiguration<ScenarioMetadataCache>
{
    public void Configure(EntityTypeBuilder<ScenarioMetadataCache> builder)
    {
        builder.ToTable("scenario_metadata_caches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.ScenarioId).IsRequired();
        builder.Property(x => x.ScenarioName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.ScenarioId }).IsUnique();

        builder.HasMany(x => x.Stages)
            .WithOne()
            .HasForeignKey("ScenarioMetadataCacheId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StageMetadataItemConfiguration : IEntityTypeConfiguration<StageMetadataItem>
{
    public void Configure(EntityTypeBuilder<StageMetadataItem> builder)
    {
        builder.ToTable("stage_metadata_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StageId).IsRequired();
        builder.Property(x => x.StageName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Order).IsRequired();

        builder.HasMany(x => x.Steps)
            .WithOne()
            .HasForeignKey("StageMetadataItemId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StepMetadataItemConfiguration : IEntityTypeConfiguration<StepMetadataItem>
{
    public void Configure(EntityTypeBuilder<StepMetadataItem> builder)
    {
        builder.ToTable("step_metadata_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StepId).IsRequired();
        builder.Property(x => x.StepName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Order).IsRequired();
    }
}
