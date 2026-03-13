namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;

public record RequestPasswordResetRequest(string Email, string Subdomain);