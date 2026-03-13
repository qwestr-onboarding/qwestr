namespace Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Requests;

public record UpdateTenantRequest(string Name, string? Subdomain);