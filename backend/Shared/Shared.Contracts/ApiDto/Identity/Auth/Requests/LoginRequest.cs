namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;

public record LoginRequest(
    string Email,
    string Password,
    string Subdomain);