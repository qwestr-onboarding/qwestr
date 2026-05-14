using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Contracts.Gamification.Events;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class BadgeEarnedEventHandler : INotificationHandler<AwardEarnedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;
    private readonly ITenantProvider _tenantProvider;
    private readonly AppUrlOptions _appUrlOptions;

    public BadgeEarnedEventHandler(INotificationDispatcher dispatcher, IUserProvider userProvider,
        ITenantProvider tenantProvider, IOptions<AppUrlOptions> appUrlOptions)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
        _tenantProvider = tenantProvider;
        _appUrlOptions = appUrlOptions.Value;
    }

    public async Task Handle(AwardEarnedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userProvider.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        var tenant = await _tenantProvider.GetByIdAsync(notification.TenantId, cancellationToken);
        var domain = "https://" + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
        var profileUrl = domain;
        if (tenant?.Subdomain is not null)
        {
            domain = "https://" + tenant.Subdomain + "." + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
            profileUrl = $"{domain}/profile";
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
                ProfileUrl = profileUrl
            },
            ActionUrl: profileUrl);

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
