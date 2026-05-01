using MediatR;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Queries.GetUnreadCount;

public sealed class GetUnreadCountQueryHandler
    : IRequestHandler<GetUnreadCountQuery, ActionResult<UnreadCountResponse>>
{
    private readonly INotificationsDbContext _db;

    public GetUnreadCountQueryHandler(INotificationsDbContext db) => _db = db;

    public async Task<ActionResult<UnreadCountResponse>> Handle(
        GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _db.Notifications
            .CountAsync(n =>
                n.TenantId == request.TenantId
                && n.RecipientUserId == request.UserId
                && !n.IsRead,
                cancellationToken);

        return ActionResult<UnreadCountResponse>.Success(new UnreadCountResponse(count));
    }
}
