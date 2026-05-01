using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Contracts.Identity.Events;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class UserInvitedEventHandler : INotificationHandler<UserInvitedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly AppUrlOptions _appUrlOptions;

    public UserInvitedEventHandler(INotificationDispatcher dispatcher, IOptions<AppUrlOptions> appUrlOptions)
    {
        _dispatcher = dispatcher;
        _appUrlOptions = appUrlOptions.Value;
    }

    public async Task Handle(UserInvitedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var activationUrl = $"{_appUrlOptions.BaseUrl}/accept-invitation?token={notification.ActivationToken}";

        var context = new NotificationContext(
            TenantId: notification.TenantId,
            RecipientUserId: Guid.Empty,
            RecipientEmail: notification.RecipientEmail,
            RecipientDisplayName: null,
            Type: NotificationType.UserInvited,
            Title: $"Вас пригласили в {notification.CompanyName}",
            Body: $"{notification.InviterName} приглашает вас в {notification.CompanyName}",
            TemplateName: "Invitation",
            TemplateModel: new
            {
                InviterName = notification.InviterName,
                CompanyName = notification.CompanyName,
                ActivationUrl = activationUrl
            },
            ActionUrl: activationUrl);

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
