namespace Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;

public sealed record NotificationPreferenceResponse(
    int Type,
    bool InAppEnabled,
    bool EmailEnabled);
