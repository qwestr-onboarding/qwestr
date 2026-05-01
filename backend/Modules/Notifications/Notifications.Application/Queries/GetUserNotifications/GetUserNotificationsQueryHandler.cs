using MediatR;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Queries.GetUserNotifications;

public sealed class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, ActionResult<NotificationListResponse>>
{
    private readonly INotificationsDbContext _db;

    public GetUserNotificationsQueryHandler(INotificationsDbContext db) => _db = db;

    public async Task<ActionResult<NotificationListResponse>> Handle(
        GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Notifications
            .Where(n => n.TenantId == request.TenantId && n.RecipientUserId == request.UserId);

        if (request.UnreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var unreadCount = await _db.Notifications
            .CountAsync(n =>
                n.TenantId == request.TenantId
                && n.RecipientUserId == request.UserId
                && !n.IsRead,
                cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(n => new NotificationResponse(
                n.Id,
                (int)n.Type,
                n.Title,
                n.Body,
                n.ActionUrl,
                n.IsRead,
                n.ReadAt,
                new DateTimeOffset(n.CreatedAt, TimeSpan.Zero)))
            .ToListAsync(cancellationToken);

        return ActionResult<NotificationListResponse>.Success(
            new NotificationListResponse(items, totalCount, unreadCount));
    }
}
