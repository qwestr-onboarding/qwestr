namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;

public record ResetPasswordRequest(string Token, string NewPassword);