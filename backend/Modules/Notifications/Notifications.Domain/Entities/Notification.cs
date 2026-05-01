using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Notifications.Domain.Entities;

public class Notification : AggregateRoot<Guid>, IAuditableEntity
{
    public Guid TenantId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string? ActionUrl { get; private set; }
    public string? PayloadJson { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset? ReadAt { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private Notification() { }

    public static ActionResult<Notification> Create(
        Guid tenantId,
        Guid recipientUserId,
        NotificationType type,
        string title,
        string body,
        string? actionUrl = null,
        string? payloadJson = null)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<Notification>.Failure("TenantId is required");
        }

        if (recipientUserId == Guid.Empty)
        {
            return ActionResult<Notification>.Failure("RecipientUserId is required");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return ActionResult<Notification>.Failure("Title is required");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return ActionResult<Notification>.Failure("Body is required");
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            RecipientUserId = recipientUserId,
            Type = type,
            Title = title.Trim(),
            Body = body.Trim(),
            ActionUrl = actionUrl?.Trim(),
            PayloadJson = payloadJson,
            IsRead = false
        };

        return ActionResult<Notification>.Success(notification);
    }

    public ActionResult MarkAsRead(DateTimeOffset readAt)
    {
        if (IsRead)
        {
            return ActionResult.Success();
        }

        IsRead = true;
        ReadAt = readAt;
        return ActionResult.Success();
    }
}