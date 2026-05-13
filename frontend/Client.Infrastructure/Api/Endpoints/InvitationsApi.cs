using Client.Infrastructure.Api.Models;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Requests;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Client.Infrastructure.Api.Endpoints;

public class InvitationsApi(QwestrApiClient api)
{
    public Task<ApiResult<CreateInvitationResponse>> CreateInvitationAsync(
        CreateInvitationRequest request, CancellationToken ct = default)
        => api.PostAsync<CreateInvitationResponse>("/api/identity/invitations", request, ct);

    public Task<ApiResult<PagedListResponse<InvitationResponse>>> GetInvitationsListAsync(
        int page, int pageSize, InvitationStatus? status = null, CancellationToken ct = default)
    {
        var url = $"/api/identity/invitations?page={page}&pageSize={pageSize}";
        if (status.HasValue)
            url += $"&status={(int)status.Value}";
        return api.GetAsync<PagedListResponse<InvitationResponse>>(url, ct);
    }

    public Task<ApiResult<AcceptInvitationResponse>> AcceptInvitationAsync(
        AcceptInvitationRequest request, CancellationToken ct = default)
        => api.PostAsync<AcceptInvitationResponse>("/api/identity/invitations/accept", request, ct);

    public Task<ApiResult<InvitationPreviewResponse>> ValidateAsync(string token, CancellationToken ct = default)
        => api.GetAsync<InvitationPreviewResponse>(
            $"/api/identity/invitations/validate?token={Uri.EscapeDataString(token)}", ct);

    public Task<ApiResult> CancelInvitationAsync(Guid invitationId, CancellationToken ct = default)
        => api.DeleteAsync($"/api/identity/invitations/{invitationId}", ct);
}