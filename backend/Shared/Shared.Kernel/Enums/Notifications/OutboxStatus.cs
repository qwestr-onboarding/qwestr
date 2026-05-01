namespace Qwestr.Shared.Kernel.Enums.Notifications;

public enum OutboxStatus
{
    Pending = 0,
    Sending = 1,
    Sent = 2,
    Failed = 3
}