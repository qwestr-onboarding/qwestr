using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Notifications.Domain.Entities;

namespace Qwestr.Modules.Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Body).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.ActionUrl).HasMaxLength(1000);
        builder.Property(x => x.PayloadJson).HasColumnType("text");
        builder.HasIndex(x => new { x.TenantId, x.RecipientUserId });
        builder.HasIndex(x => new { x.TenantId, x.RecipientUserId, x.IsRead });
    }
}
