using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
    }
}
