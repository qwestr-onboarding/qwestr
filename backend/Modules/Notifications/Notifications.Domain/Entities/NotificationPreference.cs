using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Notifications.Domain.Entities;

public class NotificationPreference : Entity<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public bool InAppEnabled { get; private set; }
    public bool EmailEnabled { get; private set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    private NotificationPreference() { }

    public static ActionResult<NotificationPreference> Create(
        Guid tenantId,
        Guid userId,
        NotificationType type,
        bool inAppEnabled,
        bool emailEnabled)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<NotificationPreference>.Failure("TenantId is required");
        }

        if (userId == Guid.Empty)
        {
            return ActionResult<NotificationPreference>.Failure("UserId is required");
        }

        var preference = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Type = type,
            InAppEnabled = inAppEnabled,
            EmailEnabled = emailEnabled
        };

        return ActionResult<NotificationPreference>.Success(preference);
    }

    public ActionResult UpdateChannels(bool inAppEnabled, bool emailEnabled)
    {
        InAppEnabled = inAppEnabled;
        EmailEnabled = emailEnabled;
        return ActionResult.Success();
    }

    public bool IsChannelEnabled(NotificationChannel channel) => channel switch
    {
        NotificationChannel.InApp => InAppEnabled,
        NotificationChannel.Email => EmailEnabled,
        _ => false
    };
}