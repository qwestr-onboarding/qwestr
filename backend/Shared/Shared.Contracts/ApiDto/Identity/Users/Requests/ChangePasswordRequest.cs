namespace Qwestr.Shared.Contracts.ApiDto.Identity.Users.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);