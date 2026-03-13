using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Modules.Identity.Domain.Enums;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetInvitationsList;

public record GetInvitationsListQuery(
    Guid TenantId,
    int Page,
    int PageSize,
    InvitationStatus? Status = null) : IRequest<ActionResult<PagedList<InvitationResponse>>>;
