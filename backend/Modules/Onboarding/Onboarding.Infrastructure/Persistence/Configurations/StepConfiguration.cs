using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class StepConfiguration : IEntityTypeConfiguration<Step>
{
    public void Configure(EntityTypeBuilder<Step> builder)
    {
        builder.Property(s => s.Title).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Content).HasColumnType("jsonb");
        builder.Property(s => s.Settings).HasColumnType("jsonb");

        builder.HasIndex(s => s.StageId);

        builder.HasOne(s => s.Stage)
            .WithMany(st => st.Steps)
            .HasForeignKey(s => s.StageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
