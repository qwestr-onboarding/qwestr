namespace Qwestr.Modules.Notifications.Application.Options;

public sealed class OutboxOptions
{
    public int PollIntervalSeconds { get; set; } = 10;
    public int BatchSize { get; set; } = 20;
    public int StuckSendingTimeoutMinutes { get; set; } = 10;
    public int RetentionDays { get; set; } = 30;
}
