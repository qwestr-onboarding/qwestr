using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Notifications.Domain.Entities;

public class OutboxMessage : AggregateRoot<Guid>, IAuditableEntity
{
    private const int MaxRetryCount = 5;

    public Guid TenantId { get; private set; }
    public NotificationType Type { get; private set; }
    public string ToAddress { get; private set; } = null!;
    public string? ToDisplayName { get; private set; }
    public string Subject { get; private set; } = null!;
    public string BodyHtml { get; private set; } = null!;
    public string? BodyText { get; private set; }
    public OutboxStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public DateTimeOffset? NextAttemptAt { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public string? LastError { get; private set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    private OutboxMessage() { }

    public static ActionResult<OutboxMessage> Create(
        Guid tenantId,
        NotificationType type,
        string toAddress,
        string? toDisplayName,
        string subject,
        string bodyHtml,
        string? bodyText,
        DateTimeOffset now)
    {
        if (tenantId == Guid.Empty)
        {
            return ActionResult<OutboxMessage>.Failure("TenantId is required");
        }

        if (string.IsNullOrWhiteSpace(toAddress))
        {
            return ActionResult<OutboxMessage>.Failure("ToAddress is required");
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            return ActionResult<OutboxMessage>.Failure("Subject is required");
        }

        if (string.IsNullOrWhiteSpace(bodyHtml))
        {
            return ActionResult<OutboxMessage>.Failure("BodyHtml is required");
        }

        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = type,
            ToAddress = toAddress.Trim(),
            ToDisplayName = toDisplayName?.Trim(),
            Subject = subject.Trim(),
            BodyHtml = bodyHtml,
            BodyText = bodyText,
            Status = OutboxStatus.Pending,
            RetryCount = 0,
            NextAttemptAt = now
        };

        return ActionResult<OutboxMessage>.Success(message);
    }

    public ActionResult MarkAsSending()
    {
        if (Status != OutboxStatus.Pending)
        {
            return ActionResult.Failure($"Cannot start sending: current status is {Status}");
        }

        Status = OutboxStatus.Sending;
        return ActionResult.Success();
    }

    public ActionResult MarkAsSent(DateTimeOffset sentAt)
    {
        if (Status != OutboxStatus.Sending)
        {
            return ActionResult.Failure($"Cannot mark as sent: current status is {Status}");
        }

        Status = OutboxStatus.Sent;
        SentAt = sentAt;
        LastError = null;
        NextAttemptAt = null;
        return ActionResult.Success();
    }

    public ActionResult MarkAttemptFailed(string error, DateTimeOffset now)
    {
        RetryCount++;
        LastError = error;

        if (RetryCount >= MaxRetryCount)
        {
            Status = OutboxStatus.Failed;
            NextAttemptAt = null;
            return ActionResult.Success();
        }

        Status = OutboxStatus.Pending;
        NextAttemptAt = now.AddSeconds(Math.Pow(2, RetryCount) * 30);
        return ActionResult.Success();
    }
}