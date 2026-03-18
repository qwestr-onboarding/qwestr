using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(q => q.QuestionText).HasMaxLength(2000).IsRequired();

        builder.HasIndex(q => q.StepId);

        builder.HasOne(q => q.Step)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.StepId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
