using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly OutboxOptions _options;

    public OutboxProcessor(
        IServiceProvider services,
        ILogger<OutboxProcessor> logger,
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
                await ProcessBatchAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_options.PollIntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in OutboxProcessor");
                await Task.Delay(TimeSpan.FromSeconds(5), CancellationToken.None);
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<INotificationsDbContext>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var now = DateTimeOffset.UtcNow;
        var messages = await db.OutboxMessages
            .Where(m => m.Status == OutboxStatus.Pending && m.NextAttemptAt <= now)
            .OrderBy(m => m.NextAttemptAt)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            var markSendingResult = message.MarkAsSending();
            if (markSendingResult.IsFailure)
            {
                _logger.LogWarning(
                    "Cannot mark outbox message {MessageId} as sending: {Error}",
                    message.Id, markSendingResult.Error);
                continue;
            }

            await db.SaveChangesAsync(ct);

            try
            {
                await emailSender.SendAsync(
                    message.ToAddress,
                    message.ToDisplayName,
                    message.Subject,
                    message.BodyHtml,
                    message.BodyText,
                    ct);

                message.MarkAsSent(DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send outbox message {MessageId}", message.Id);
                message.MarkAttemptFailed(ex.Message, DateTimeOffset.UtcNow);
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
