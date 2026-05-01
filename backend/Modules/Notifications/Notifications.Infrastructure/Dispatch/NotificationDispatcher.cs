using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Models;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Modules.Notifications.Domain.Entities;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Infrastructure.Dispatch;

public sealed class NotificationDispatcher : INotificationDispatcher
{
    private readonly INotificationsDbContext _db;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly ISignalRNotificationDispatcher _signalR;
    private readonly NotificationDefaultsOptions _defaults;
    private readonly NotificationThrottlingOptions _throttling;

    public NotificationDispatcher(
        INotificationsDbContext db,
        IEmailTemplateRenderer templateRenderer,
        ISignalRNotificationDispatcher signalR,
        IOptions<NotificationDefaultsOptions> defaults,
        IOptions<NotificationThrottlingOptions> throttling)
    {
        _db = db;
        _templateRenderer = templateRenderer;
        _signalR = signalR;
        _defaults = defaults.Value;
        _throttling = throttling.Value;
    }

    public async Task<ActionResult> DispatchAsync(NotificationContext context, CancellationToken cancellationToken = default)
    {
        var preference = await _db.NotificationPreferences
            .FirstOrDefaultAsync(p =>
                p.TenantId == context.TenantId
                && p.UserId == context.RecipientUserId
                && p.Type == context.Type,
                cancellationToken);

        var defaults = _defaults.GetDefaults(context.Type);
        var inAppEnabled = preference?.InAppEnabled ?? defaults.InApp;
        var emailEnabled = preference?.EmailEnabled ?? defaults.Email;

        var channels = new List<NotificationChannel>();
        if (inAppEnabled)
        {
            channels.Add(NotificationChannel.InApp);
        }
        if (emailEnabled)
        {
            channels.Add(NotificationChannel.Email);
        }

        var throttleWindow = _throttling.GetThrottleWindow(context.Type);
        if (throttleWindow.HasValue)
        {
            var windowStart = DateTimeOffset.UtcNow - throttleWindow.Value;
            var recentChannels = await _db.NotificationDispatchLogs
                .Where(l =>
                    l.TenantId == context.TenantId
                    && l.UserId == context.RecipientUserId
                    && l.Type == context.Type
                    && l.DispatchedAt >= windowStart)
                .Select(l => l.Channel)
                .ToListAsync(cancellationToken);

            channels = channels.Where(c => !recentChannels.Contains(c)).ToList();
        }

        if (channels.Count == 0)
        {
            return ActionResult.Success();
        }

        var now = DateTimeOffset.UtcNow;
        NotificationResponse? signalRDto = null;

        if (channels.Contains(NotificationChannel.InApp))
        {
            var createResult = Notification.Create(
                context.TenantId,
                context.RecipientUserId,
                context.Type,
                context.Title,
                context.Body,
                context.ActionUrl);

            if (createResult.IsFailure)
            {
                return ActionResult.Failure(createResult.Error!);
            }

            _db.Notifications.Add(createResult.Value!);

            signalRDto = new NotificationResponse(
                createResult.Value!.Id,
                (int)context.Type,
                context.Title,
                context.Body,
                context.ActionUrl,
                IsRead: false,
                ReadAt: null,
                CreatedAt: now);

            _db.NotificationDispatchLogs.Add(NotificationDispatchLog.Create(
                context.TenantId, context.RecipientUserId, context.Type, NotificationChannel.InApp, now));
        }

        if (channels.Contains(NotificationChannel.Email))
        {
            var rendered = await _templateRenderer.RenderAsync(
                context.TemplateName, context.TemplateModel, cancellationToken);

            var outboxResult = OutboxMessage.Create(
                context.TenantId,
                context.Type,
                context.RecipientEmail,
                context.RecipientDisplayName,
                rendered.Subject,
                rendered.BodyHtml,
                rendered.BodyText,
                now);

            if (outboxResult.IsFailure)
            {
                return ActionResult.Failure(outboxResult.Error!);
            }

            _db.OutboxMessages.Add(outboxResult.Value!);

            _db.NotificationDispatchLogs.Add(NotificationDispatchLog.Create(
                context.TenantId, context.RecipientUserId, context.Type, NotificationChannel.Email, now));
        }

        await _db.SaveChangesAsync(cancellationToken);

        if (signalRDto is not null)
        {
            await _signalR.PushAsync(context.RecipientUserId, signalRDto, cancellationToken);
        }

        return ActionResult.Success();
    }
}
