using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Requests;
using Qwestr.Shared.Contracts.ApiDto.Notifications.Responses;

namespace Client.Infrastructure.Api.Endpoints;

public class NotificationsApi(QwestrApiClient api)
{
    public Task<ApiResult<List<NotificationPreferenceResponse>>> GetPreferencesAsync(CancellationToken ct = default)
        => api.GetAsync<List<NotificationPreferenceResponse>>("/api/notifications/preferences", ct);

    public Task<ApiResult> UpdatePreferencesAsync(
        UpdatePreferencesRequest request, CancellationToken ct = default)
        => api.PutAsync("/api/notifications/preferences", request, ct);
}
