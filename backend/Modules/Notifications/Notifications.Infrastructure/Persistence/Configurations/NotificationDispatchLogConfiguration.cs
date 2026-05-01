using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Notifications.Domain.Entities;

namespace Qwestr.Modules.Notifications.Infrastructure.Persistence.Configurations;

public sealed class NotificationDispatchLogConfiguration : IEntityTypeConfiguration<NotificationDispatchLog>
{
    public void Configure(EntityTypeBuilder<NotificationDispatchLog> builder)
    {
        builder.ToTable("notification_dispatch_logs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.Type, x.Channel, x.DispatchedAt });
        builder.HasIndex(x => x.DispatchedAt);
    }
}
