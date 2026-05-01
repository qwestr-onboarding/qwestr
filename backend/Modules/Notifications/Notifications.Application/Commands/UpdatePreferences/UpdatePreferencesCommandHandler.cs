using MediatR;
using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Notifications.Application.Abstractions;
using Qwestr.Modules.Notifications.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Notifications.Application.Commands.UpdatePreferences;

public sealed class UpdatePreferencesCommandHandler : IRequestHandler<UpdatePreferencesCommand, ActionResult>
{
    private readonly INotificationsDbContext _db;

    public UpdatePreferencesCommandHandler(INotificationsDbContext db) => _db = db;

    public async Task<ActionResult> Handle(UpdatePreferencesCommand request, CancellationToken cancellationToken)
    {
        var types = request.Preferences.Select(p => p.Type).ToList();
        var existing = await _db.NotificationPreferences
            .Where(p =>
                p.TenantId == request.TenantId
                && p.UserId == request.UserId
                && types.Contains(p.Type))
            .ToListAsync(cancellationToken);

        foreach (var item in request.Preferences)
        {
            var preference = existing.FirstOrDefault(p => p.Type == item.Type);
            if (preference is not null)
            {
                preference.UpdateChannels(item.InAppEnabled, item.EmailEnabled);
            }
            else
            {
                var result = NotificationPreference.Create(
                    request.TenantId, request.UserId, item.Type, item.InAppEnabled, item.EmailEnabled);

                if (result.IsFailure)
                {
                    return ActionResult.Failure(result.Error!);
                }

                _db.NotificationPreferences.Add(result.Value!);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return ActionResult.Success();
    }
}
