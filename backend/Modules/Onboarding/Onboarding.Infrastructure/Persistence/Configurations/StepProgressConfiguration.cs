using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class StepProgressConfiguration : IEntityTypeConfiguration<StepProgress>
{
    public void Configure(EntityTypeBuilder<StepProgress> builder)
    {
        builder.Property(sp => sp.CheckedItems).HasColumnType("jsonb");

        // Уникальный прогресс на каждый шаг в рамках одного назначения
        builder.HasIndex(sp => new { sp.AssignmentId, sp.StepId }).IsUnique();

        builder.HasOne(sp => sp.Assignment)
            .WithMany(a => a.StepProgresses)
            .HasForeignKey(sp => sp.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
