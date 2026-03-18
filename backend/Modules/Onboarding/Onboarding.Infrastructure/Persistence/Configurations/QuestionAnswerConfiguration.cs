using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class QuestionAnswerConfiguration : IEntityTypeConfiguration<QuestionAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
    {
        // List<Guid>? → uuid[] (Npgsql native array support)
        builder.Property(qa => qa.SelectedOptionIds).HasColumnType("uuid[]");

        builder.HasIndex(qa => qa.QuestionId);

        // Для шага типа Question: ответ привязан напрямую к StepProgress
        builder.HasOne(qa => qa.StepProgress)
            .WithMany(sp => sp.QuestionAnswers)
            .HasForeignKey(qa => qa.StepProgressId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Для шага типа Test: ответ привязан к TestAttempt
        builder.HasOne(qa => qa.TestAttempt)
            .WithMany(ta => ta.QuestionAnswers)
            .HasForeignKey(qa => qa.TestAttemptId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
