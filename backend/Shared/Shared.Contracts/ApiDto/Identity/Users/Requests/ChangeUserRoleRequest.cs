using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Requests;

public record ChangeUserRoleRequest(UserRole NewRole);