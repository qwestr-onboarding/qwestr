using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.Options;

public sealed class NotificationDefaultsOptions
{
    public ChannelDefault UserInvited { get; set; } = new() { InApp = false, Email = true };
    public ChannelDefault AccountActivated { get; set; } = new() { InApp = true, Email = true };
    public ChannelDefault ScenarioAssigned { get; set; } = new() { InApp = true, Email = true };
    public ChannelDefault ScenarioCompleted { get; set; } = new() { InApp = true, Email = false };
    public ChannelDefault StepCompleted { get; set; } = new() { InApp = false, Email = false };
    public ChannelDefault ScenarioReminderInactive { get; set; } = new() { InApp = true, Email = true };
    public ChannelDefault BadgeEarned { get; set; } = new() { InApp = true, Email = false };
    public ChannelDefault LevelUp { get; set; } = new() { InApp = true, Email = false };
    public ChannelDefault LeaderboardPositionChanged { get; set; } = new() { InApp = true, Email = false };

    public ChannelDefault GetDefaults(NotificationType type) => type switch
    {
        NotificationType.UserInvited => UserInvited,
        NotificationType.AccountActivated => AccountActivated,
        NotificationType.ScenarioAssigned => ScenarioAssigned,
        NotificationType.ScenarioCompleted => ScenarioCompleted,
        NotificationType.StepCompleted => StepCompleted,
        NotificationType.ScenarioReminderInactive => ScenarioReminderInactive,
        NotificationType.BadgeEarned => BadgeEarned,
        NotificationType.LevelUp => LevelUp,
        NotificationType.LeaderboardPositionChanged => LeaderboardPositionChanged,
        _ => new ChannelDefault { InApp = true, Email = false }
    };
}

public sealed class ChannelDefault
{
    public bool InApp { get; set; }
    public bool Email { get; set; }
}
