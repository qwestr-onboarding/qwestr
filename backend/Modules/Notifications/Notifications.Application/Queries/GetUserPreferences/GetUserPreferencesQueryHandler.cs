using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Application.Options;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.Queries.GetUserPreferences;

public sealed class GetUserPreferencesQueryHandler
    : IRequestHandler<GetUserPreferencesQuery, ActionResult<IReadOnlyList<NotificationPreferenceResponse>>>
{
    private readonly INotificationsDbContext _db;
    private readonly NotificationDefaultsOptions _defaults;

    public GetUserPreferencesQueryHandler(
        INotificationsDbContext db,
        IOptions<NotificationDefaultsOptions> defaults)
    {
        _db = db;
        _defaults = defaults.Value;
    }

    public async Task<ActionResult<IReadOnlyList<NotificationPreferenceResponse>>> Handle(
        GetUserPreferencesQuery request, CancellationToken cancellationToken)
    {
        var stored = await _db.NotificationPreferences
            .Where(p => p.TenantId == request.TenantId && p.UserId == request.UserId)
            .ToDictionaryAsync(p => p.Type, cancellationToken);

        var result = Enum.GetValues<NotificationType>()
            .Select(type =>
            {
                if (stored.TryGetValue(type, out var pref))
                {
                    return new NotificationPreferenceResponse((int)type, pref.InAppEnabled, pref.EmailEnabled);
                }

                var d = _defaults.GetDefaults(type);
                return new NotificationPreferenceResponse((int)type, d.InApp, d.Email);
            })
            .ToList();

        return ActionResult<IReadOnlyList<NotificationPreferenceResponse>>.Success(result);
    }
}
