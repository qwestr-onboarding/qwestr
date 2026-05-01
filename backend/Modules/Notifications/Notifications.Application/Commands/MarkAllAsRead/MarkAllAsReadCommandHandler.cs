using MediatR;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Commands.MarkAllAsRead;

public sealed class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, ActionResult>
{
    private readonly INotificationsDbContext _db;

    public MarkAllAsReadCommandHandler(INotificationsDbContext db) => _db = db;

    public async Task<ActionResult> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var unread = await _db.Notifications
            .Where(n =>
                n.TenantId == request.TenantId
                && n.RecipientUserId == request.UserId
                && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
        {
            notification.MarkAsRead(now);
        }

        if (unread.Count > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        return ActionResult.Success();
    }
}
