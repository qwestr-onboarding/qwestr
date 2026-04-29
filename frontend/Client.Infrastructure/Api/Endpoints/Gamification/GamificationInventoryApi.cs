using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

namespace Client.Infrastructure.Api.Endpoints.Gamification;

public class GamificationInventoryApi(QwestrApiClient api)
{
    public Task<ApiResult<InventoryListResponse>> GetMyInventoryAsync(CancellationToken ct = default)
        => api.GetAsync<InventoryListResponse>("/api/gamification/inventory/me", ct);

    public Task<ApiResult> EquipItemAsync(EquipItemRequest request, CancellationToken ct = default)
        => api.PostAsync("/api/gamification/inventory/equip", request, ct);

    public Task<ApiResult> UnequipItemAsync(UnequipItemRequest request, CancellationToken ct = default)
        => api.PostAsync("/api/gamification/inventory/unequip", request, ct);
}
