using MediatR;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Notifications;

namespace Qwestr.Modules.Notifications.Application.Commands.UpdatePreferences;

public sealed record UpdatePreferencesCommand(
    Guid TenantId,
    Guid UserId,
    IReadOnlyList<PreferenceItem> Preferences) : IRequest<ActionResult>;

public sealed record PreferenceItem(NotificationType Type, bool InAppEnabled, bool EmailEnabled);
