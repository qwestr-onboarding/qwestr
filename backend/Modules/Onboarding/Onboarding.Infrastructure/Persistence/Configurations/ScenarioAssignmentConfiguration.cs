using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Onboarding.Domain.Entities;

namespace Qwestr.Modules.Onboarding.Infrastructure.Persistence.Configurations;

public class ScenarioAssignmentConfiguration : IEntityTypeConfiguration<ScenarioAssignment>
{
    public void Configure(EntityTypeBuilder<ScenarioAssignment> builder)
    {
        // Основные запросы: все назначения сотрудника, все назначения по сценарию
        builder.HasIndex(a => new { a.TenantId, a.UserId });
        builder.HasIndex(a => a.ScenarioId);
    }
}
