using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Queries.GetUserPreferences;

public sealed record GetUserPreferencesQuery(
    Guid TenantId,
    Guid UserId) : IRequest<ActionResult<IReadOnlyList<NotificationPreferenceResponse>>>;
