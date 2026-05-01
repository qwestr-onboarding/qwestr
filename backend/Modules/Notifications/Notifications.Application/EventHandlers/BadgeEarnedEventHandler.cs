using MediatR;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class BadgeEarnedEventHandler : INotificationHandler<AwardEarnedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;

    public BadgeEarnedEventHandler(INotificationDispatcher dispatcher, IUserProvider userProvider)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
    }

    public async Task Handle(AwardEarnedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userProvider.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        var displayName = $"{user.FirstName} {user.LastName}".Trim();

        var context = new NotificationContext(
            TenantId: notification.TenantId,
            RecipientUserId: notification.UserId,
            RecipientEmail: user.Email,
            RecipientDisplayName: displayName,
            Type: NotificationType.BadgeEarned,
            Title: $"Вы получили награду: {notification.AwardTitle}",
            Body: $"Вы заработали награду «{notification.AwardTitle}»!",
            TemplateName: "BadgeEarned",
            TemplateModel: new
            {
                UserName = displayName,
                BadgeName = notification.AwardTitle,
                BadgeDescription = notification.AwardDescription ?? string.Empty,
                ProfileUrl = "/profile"
            },
            ActionUrl: "/profile");

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
