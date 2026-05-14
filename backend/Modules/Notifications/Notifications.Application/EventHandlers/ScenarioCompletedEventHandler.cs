using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class ScenarioCompletedEventHandler
    : INotificationHandler<OnboardingScenarioCompletedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;
    private readonly ITenantProvider _tenantProvider;
    private readonly AppUrlOptions _appUrlOptions;

    public ScenarioCompletedEventHandler(INotificationDispatcher dispatcher, IUserProvider userProvider, 
        ITenantProvider tenantProvider, IOptions<AppUrlOptions> appUrlOptions)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
        _tenantProvider = tenantProvider;
        _appUrlOptions = appUrlOptions.Value;
    }

    public async Task Handle(OnboardingScenarioCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userProvider.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        var tenant = await _tenantProvider.GetByIdAsync(notification.TenantId, cancellationToken);
        var domain = "https://" + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
        var actionUrl = domain;
        if (tenant?.Subdomain is not null)
        {
            domain = "https://" + tenant.Subdomain + "." + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
            actionUrl = $"{domain}/onboarding";
        }
        
        var displayName = $"{user.FirstName} {user.LastName}".Trim();

        var context = new NotificationContext(
            TenantId: notification.TenantId,
            RecipientUserId: notification.UserId,
            RecipientEmail: user.Email,
            RecipientDisplayName: displayName,
            Type: NotificationType.ScenarioCompleted,
            Title: "Сценарий адаптации завершён",
            Body: $"Вы успешно завершили сценарий адаптации «{notification.ScenarioTitle}»!",
            TemplateName: "ScenarioCompleted",
            TemplateModel: new
            {
                UserName = displayName,
                ScenarioName = notification.ScenarioTitle,
                TotalPoints = notification.TotalPoints.ToString()
            },
            ActionUrl: actionUrl);

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
