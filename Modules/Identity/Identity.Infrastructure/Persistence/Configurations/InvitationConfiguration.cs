using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Infrastructure.Persistence.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasIndex(i => i.Token).IsUnique();
        builder.HasIndex(i => new { i.TenantId, i.Email });
    }
}
