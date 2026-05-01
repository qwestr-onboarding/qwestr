namespace Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;

public sealed record NotificationResponse(
    Guid Id,
    int Type,
    string Title,
    string Body,
    string? ActionUrl,
    bool IsRead,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt);
