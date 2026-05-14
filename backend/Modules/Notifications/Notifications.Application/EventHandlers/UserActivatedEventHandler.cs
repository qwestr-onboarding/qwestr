using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Contracts.Identity.Events;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class UserActivatedEventHandler : INotificationHandler<UserActivatedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly AppUrlOptions _appUrlOptions;

    public UserActivatedEventHandler(INotificationDispatcher dispatcher, IOptions<AppUrlOptions> appUrlOptions)
    {
        _dispatcher = dispatcher;
        _appUrlOptions = appUrlOptions.Value;
    }

    public async Task Handle(UserActivatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var displayName = $"{notification.FirstName} {notification.LastName}".Trim();

        var domain = "https://" + notification.TenantSubdomain + "." + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
        var appUrl = $"{domain}/dashboard";
        var context = new NotificationContext(
            TenantId: notification.TenantId,
            RecipientUserId: notification.UserId,
            RecipientEmail: notification.Email,
            RecipientDisplayName: displayName,
            Type: NotificationType.AccountActivated,
            Title: $"Добро пожаловать в {notification.CompanyName}!",
            Body: $"Ваш аккаунт в {notification.CompanyName} активирован. Добро пожаловать!",
            TemplateName: "Welcome",
            TemplateModel: new
            {
                UserName = displayName,
                CompanyName = notification.CompanyName,
                AppUrl = appUrl
            },
            ActionUrl: appUrl);

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
