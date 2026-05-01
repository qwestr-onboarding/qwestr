using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Commands.MarkAllAsRead;

public sealed record MarkAllAsReadCommand(
    Guid TenantId,
    Guid UserId) : IRequest<ActionResult>;
