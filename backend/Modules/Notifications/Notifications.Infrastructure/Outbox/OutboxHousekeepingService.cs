using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Infrastructure.Outbox;

public sealed class OutboxHousekeepingService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OutboxHousekeepingService> _logger;
    private readonly OutboxOptions _options;

    public OutboxHousekeepingService(
        IServiceProvider services,
        ILogger<OutboxHousekeepingService> logger,
        IOptions<OutboxOptions> options)
    {
        _services = services;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                await RecoverStuckMessagesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                await CleanupOldRecordsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in OutboxHousekeepingService");
            }
        }
    }

    private async Task RecoverStuckMessagesAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<INotificationsDbContext>();

        var stuckThreshold = DateTime.UtcNow.AddMinutes(-_options.StuckSendingTimeoutMinutes);
        var stuck = await db.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Sending && m.UpdatedAt <= stuckThreshold)
            .ToListAsync(ct);

        foreach (var message in stuck)
        {
            _logger.LogWarning("Recovering stuck outbox message {MessageId}", message.Id);
            message.MarkAttemptFailed("Recovered from stuck Sending state", DateTimeOffset.UtcNow);
        }

        if (stuck.Count > 0)
        {
            await db.SaveChangesAsync(ct);
        }
    }

    private async Task CleanupOldRecordsAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<INotificationsDbContext>();

        var cutoff = DateTime.UtcNow.AddDays(-_options.RetentionDays);

        var oldMessages = await db.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Sent && m.UpdatedAt <= cutoff)
            .ToListAsync(ct);

        if (oldMessages.Count > 0)
        {
            db.OutboxMessages.RemoveRange(oldMessages);
        }

        var oldLogs = await db.NotificationDispatchLogs
            .Where(l => l.DispatchedAt.DateTime <= cutoff)
            .ToListAsync(ct);

        if (oldLogs.Count > 0)
        {
            db.NotificationDispatchLogs.RemoveRange(oldLogs);
        }

        if (oldMessages.Count + oldLogs.Count > 0)
        {
            _logger.LogInformation(
                "Cleaned up {MessageCount} outbox messages and {LogCount} dispatch logs",
                oldMessages.Count, oldLogs.Count);
            await db.SaveChangesAsync(ct);
        }
    }
}
