using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Configurations;

public class StepCompletionFactConfiguration : IEntityTypeConfiguration<StepCompletionFact>
{
    public void Configure(EntityTypeBuilder<StepCompletionFact> builder)
    {
        builder.ToTable("step_completion_facts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.AssignmentId).IsRequired();
        builder.Property(x => x.ScenarioId).IsRequired();
        builder.Property(x => x.StageId).IsRequired();
        builder.Property(x => x.StepId).IsRequired();
        builder.Property(x => x.StepType).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CompletedAt).IsRequired();
        builder.Property(x => x.TimeSpent).IsRequired();
        builder.Property(x => x.WasSuccessful).IsRequired();
        builder.Property(x => x.AttemptNumber).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.CompletedAt });
        builder.HasIndex(x => new { x.TenantId, x.StepId });
        builder.HasIndex(x => new { x.TenantId, x.ScenarioId, x.CompletedAt });

        builder.HasIndex(x => new { x.TenantId, x.StepId, x.UserId, x.AttemptNumber })
            .IsUnique();
    }
}
