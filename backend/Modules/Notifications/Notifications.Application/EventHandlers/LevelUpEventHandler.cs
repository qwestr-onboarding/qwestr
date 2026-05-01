using MediatR;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class LevelUpEventHandler : INotificationHandler<PlayerLevelUpIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;

    public LevelUpEventHandler(INotificationDispatcher dispatcher, IUserProvider userProvider)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
    }

    public async Task Handle(PlayerLevelUpIntegrationEvent notification, CancellationToken cancellationToken)
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
            Type: NotificationType.LevelUp,
            Title: "Новый уровень!",
            Body: $"Поздравляем! Вы достигли уровня {notification.NewLevel}",
            TemplateName: "LevelUp",
            TemplateModel: new
            {
                UserName = displayName,
                NewLevel = notification.NewLevel,
                ProfileUrl = "/profile"
            },
            ActionUrl: "/profile");

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
