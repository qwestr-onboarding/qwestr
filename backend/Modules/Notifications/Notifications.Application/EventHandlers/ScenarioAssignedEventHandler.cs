using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class ScenarioAssignedEventHandler
    : INotificationHandler<OnboardingScenarioAssignedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;
    private readonly ITenantProvider _tenantProvider;
    private readonly AppUrlOptions _appUrlOptions;

    public ScenarioAssignedEventHandler(INotificationDispatcher dispatcher,
        IUserProvider userProvider, ITenantProvider tenantProvider, IOptions<AppUrlOptions> appUrlOptions)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
        _tenantProvider = tenantProvider;
        _appUrlOptions = appUrlOptions.Value;
    }

    public async Task Handle(OnboardingScenarioAssignedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var user = await _userProvider.GetByIdAsync(notification.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        var tenant = await _tenantProvider.GetByIdAsync(notification.TenantId, cancellationToken);
        var domain = "https://" + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
        var scenarioUrl = domain;
        if (tenant?.Subdomain is not null)
        {
            domain = "https://" + tenant.Subdomain + "." + _appUrlOptions.BaseUrl.Replace("https://", "").Replace("http://", "");
            scenarioUrl = $"{domain}/onboarding";
        }
        
        var displayName = $"{user.FirstName} {user.LastName}".Trim();

        var context = new NotificationContext(
            TenantId: notification.TenantId,
            RecipientUserId: notification.UserId,
            RecipientEmail: user.Email,
            RecipientDisplayName: displayName,
            Type: NotificationType.ScenarioAssigned,
            Title: "Назначен сценарий адаптации",
            Body: $"Вам назначен сценарий: {notification.ScenarioTitle}",
            TemplateName: "ScenarioAssigned",
            TemplateModel: new
            {
                UserName = displayName,
                ScenarioName = notification.ScenarioTitle,
                ScenarioUrl = scenarioUrl
            },
            ActionUrl: scenarioUrl);

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
