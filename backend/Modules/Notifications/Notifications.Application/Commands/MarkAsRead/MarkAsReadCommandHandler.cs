using MediatR;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Commands.MarkAsRead;

public sealed class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, ActionResult>
{
    private readonly INotificationsDbContext _db;

    public MarkAsReadCommandHandler(INotificationsDbContext db) => _db = db;

    public async Task<ActionResult> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == request.NotificationId
                && n.TenantId == request.TenantId
                && n.RecipientUserId == request.UserId,
                cancellationToken);

        if (notification is null)
        {
            return ActionResult.Failure("notifications.notification.not_found");
        }

        notification.MarkAsRead(DateTimeOffset.UtcNow);
        await _db.SaveChangesAsync(cancellationToken);

        return ActionResult.Success();
    }
}
