using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Application.DTOs;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetUsersList;

public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, ActionResult<PagedList<UserListItemResponse>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersListQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ActionResult<PagedList<UserListItemResponse>>> Handle(GetUsersListQuery request, CancellationToken ct)
    {
        var users = await _userRepository.GetActiveByTenantAsync(request.TenantId, request.Page, request.PageSize, ct);
        var totalCount = await _userRepository.CountByTenantAsync(request.TenantId, ct);

        var items = users.Select(u => new UserListItemResponse(
            u.Id,
            u.Email,
            $"{u.FirstName} {u.LastName}",
            u.Role,
            u.IsActive,
            u.LastLoginAt)).ToList();

        return ActionResult<PagedList<UserListItemResponse>>.Success(
            new PagedList<UserListItemResponse>(items, request.Page, request.PageSize, totalCount));
    }
}
