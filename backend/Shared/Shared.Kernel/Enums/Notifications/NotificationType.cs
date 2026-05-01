namespace Qwestr.Shared.Kernel.Enums.Notifications;

public enum NotificationType
{
    // Identity
    UserInvited = 100,
    AccountActivated = 101,

    // Onboarding
    ScenarioAssigned = 200,
    ScenarioCompleted = 201,
    StepCompleted = 202,
    ScenarioReminderInactive = 203,

    // Gamification
    BadgeEarned = 300,
    LevelUp = 301,
    LeaderboardPositionChanged = 302
}