using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Application.Queries.GetInvitationsList;

public record GetInvitationsListQuery(
    Guid TenantId,
    int Page,
    int PageSize,
    InvitationStatus? Status = null) : IRequest<ActionResult<PagedList<InvitationResponse>>>;
