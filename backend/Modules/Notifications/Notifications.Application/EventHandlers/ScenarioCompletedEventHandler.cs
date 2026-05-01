using MediatR;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Shared.Contracts.Identity;
using Qwestr.Shared.Contracts.Onboarding.Events;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.EventHandlers;

public sealed class ScenarioCompletedEventHandler
    : INotificationHandler<OnboardingScenarioCompletedIntegrationEvent>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUserProvider _userProvider;

    public ScenarioCompletedEventHandler(INotificationDispatcher dispatcher, IUserProvider userProvider)
    {
        _dispatcher = dispatcher;
        _userProvider = userProvider;
    }

    public async Task Handle(OnboardingScenarioCompletedIntegrationEvent notification, CancellationToken cancellationToken)
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
            ActionUrl: "/onboarding");

        await _dispatcher.DispatchAsync(context, cancellationToken);
    }
}
