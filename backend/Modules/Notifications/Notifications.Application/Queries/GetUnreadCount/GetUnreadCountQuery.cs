using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery(
    Guid TenantId,
    Guid UserId) : IRequest<ActionResult<UnreadCountResponse>>;
