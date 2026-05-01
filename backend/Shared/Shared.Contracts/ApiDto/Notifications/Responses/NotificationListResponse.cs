namespace Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;

public sealed record NotificationListResponse(
    IReadOnlyList<NotificationResponse> Items,
    int TotalCount,
    int UnreadCount);
