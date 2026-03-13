using MediatR;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Shared.Contracts.ApiDto.Identity.Users.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ActionResult<UserProfileResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ActionResult<UserProfileResponse>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.TargetUserId, ct);

        if (user is null || user.TenantId != request.TenantId)
        {
            return ActionResult<UserProfileResponse>.Failure("User not found.");
        }

        var response = new UserProfileResponse(
            user.Id,
            user.TenantId,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.IsActive,
            user.TwoFactorEnabled,
            user.LastLoginAt,
            user.CreatedAt);

        return ActionResult<UserProfileResponse>.Success(response);
    }
}
