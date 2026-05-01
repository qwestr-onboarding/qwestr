namespace Qwestr.Shared.Contracts.ApiDto.Notifications.Requests;

public sealed record UpdatePreferencesRequest(
    IReadOnlyList<UpdatePreferenceItemRequest> Preferences);

public sealed record UpdatePreferenceItemRequest(
    int Type,
    bool InAppEnabled,
    bool EmailEnabled);
