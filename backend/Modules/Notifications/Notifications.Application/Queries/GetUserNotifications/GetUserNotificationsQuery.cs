using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Queries.GetUserNotifications;

public sealed record GetUserNotificationsQuery(
    Guid TenantId,
    Guid UserId,
    bool UnreadOnly,
    int Take,
    int Skip) : IRequest<ActionResult<NotificationListResponse>>;
