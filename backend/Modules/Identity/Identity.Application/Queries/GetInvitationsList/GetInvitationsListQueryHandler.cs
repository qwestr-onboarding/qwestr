using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetInvitationsList;

public class GetInvitationsListQueryHandler : IRequestHandler<GetInvitationsListQuery, ActionResult<PagedList<InvitationResponse>>>
{
    private readonly IInvitationRepository _invitationRepository;

    public GetInvitationsListQueryHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<ActionResult<PagedList<InvitationResponse>>> Handle(GetInvitationsListQuery request, CancellationToken ct)
    {
        var invitations = await _invitationRepository.GetByTenantAsync(request.TenantId, request.Page, request.PageSize, request.Status, ct);
        var totalCount = await _invitationRepository.CountByTenantAsync(request.TenantId, request.Status, ct);

        var items = invitations.Select(i => new InvitationResponse(
            i.Id,
            i.Email,
            i.Role,
            i.Status,
            $"{i.InvitedByUser.FirstName} {i.InvitedByUser.LastName}",
            i.CreatedAt,
            i.ExpiresAt)).ToList();

        return ActionResult<PagedList<InvitationResponse>>.Success(
            new PagedList<InvitationResponse>(items, request.Page, request.PageSize, totalCount));
    }
}
