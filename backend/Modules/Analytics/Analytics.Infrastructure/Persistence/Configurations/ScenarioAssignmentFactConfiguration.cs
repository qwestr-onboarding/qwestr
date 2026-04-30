using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Analytics.Domain.Entities.Facts;

namespace Qwestr.Modules.Analytics.Infrastructure.Persistence.Configurations;

public class ScenarioAssignmentFactConfiguration : IEntityTypeConfiguration<ScenarioAssignmentFact>
{
    public void Configure(EntityTypeBuilder<ScenarioAssignmentFact> builder)
    {
        builder.ToTable("scenario_assignment_facts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.AssignmentId).IsRequired();
        builder.Property(x => x.ScenarioId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.AssignedAt).IsRequired();
        builder.Property(x => x.CurrentStatus).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.AssignedAt });
        builder.HasIndex(x => new { x.TenantId, x.AssignmentId }).IsUnique();
    }
}
