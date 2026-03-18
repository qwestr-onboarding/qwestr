using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder.HasIndex(ta => ta.StepProgressId);

        builder.HasOne(ta => ta.StepProgress)
            .WithMany(sp => sp.TestAttempts)
            .HasForeignKey(ta => ta.StepProgressId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
