using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Commands.MarkAsRead;

public sealed record MarkAsReadCommand(
    Guid TenantId,
    Guid UserId,
    Guid NotificationId) : IRequest<ActionResult>;
