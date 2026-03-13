using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetUsersList;

public record GetUsersListQuery(
    Guid TenantId,
    int Page,
    int PageSize) : IRequest<ActionResult<PagedList<UserListItemResponse>>>;
