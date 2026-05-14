using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class AiGenerationQuotaConfiguration : IEntityTypeConfiguration<AiGenerationQuota>
{
    public void Configure(EntityTypeBuilder<AiGenerationQuota> builder)
    {
        builder.HasIndex(q => new { q.TenantId, q.Date }).IsUnique();
    }
}
