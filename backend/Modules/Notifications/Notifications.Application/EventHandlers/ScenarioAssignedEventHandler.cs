using MediatR;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class ScenarioAssignedEventHandler
    : INotificationHandler<OnboardingScenarioAssignedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;

    public ScenarioAssignedEventHandler(INotificationDispatcher dispatcher, IUserProvider userProvider)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
    }

    public async Task Handle(OnboardingScenarioAssignedIntegrationEvent notification, CancellationToken cancellationToken)
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
            Type: NotificationType.ScenarioAssigned,
            Title: "Назначен сценарий адаптации",
            Body: $"Вам назначен сценарий: {notification.ScenarioTitle}",
            TemplateName: "ScenarioAssigned",
            TemplateModel: new
            {
                UserName = displayName,
                ScenarioName = notification.ScenarioTitle,
                ScenarioUrl = "/onboarding"
            },
            ActionUrl: "/onboarding");

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
