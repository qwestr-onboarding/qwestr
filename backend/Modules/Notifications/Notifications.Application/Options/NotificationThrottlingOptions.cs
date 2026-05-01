using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.Options;

public sealed class NotificationThrottlingOptions
{
    public TimeSpan? ScenarioReminderInactive { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan? LeaderboardPositionChanged { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan? GetThrottleWindow(NotificationType type) => type switch
    {
        NotificationType.ScenarioReminderInactive => ScenarioReminderInactive,
        NotificationType.LeaderboardPositionChanged => LeaderboardPositionChanged,
        _ => null
    };
}
