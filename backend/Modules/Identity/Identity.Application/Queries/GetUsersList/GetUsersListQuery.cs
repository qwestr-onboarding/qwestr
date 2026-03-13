using MediatR;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetUsersList;

public record GetUsersListQuery(
    Guid TenantId,
    int Page,
    int PageSize) : IRequest<ActionResult<PagedList<UserListItemResponse>>>;
