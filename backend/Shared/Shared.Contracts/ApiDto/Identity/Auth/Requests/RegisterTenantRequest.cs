namespace Qwestr.Shared.Contracts.ApiDto.Identity.Auth.Requests;

public record RegisterTenantRequest(
    string Name,
    string? Subdomain,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerFirstName,
    string OwnerLastName);