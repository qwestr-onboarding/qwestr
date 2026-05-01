using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.Models;

public sealed record NotificationContext(
    Guid TenantId,
    Guid RecipientUserId,
    string RecipientEmail,
    string? RecipientDisplayName,
    NotificationType Type,
    string Title,
    string Body,
    string TemplateName,
    object TemplateModel,
    string? ActionUrl = null);
