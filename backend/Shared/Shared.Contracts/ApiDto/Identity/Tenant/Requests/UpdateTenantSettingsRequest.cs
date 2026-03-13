namespace Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Requests;

public record UpdateTenantSettingsRequest(string? Timezone, string? Language, string? CompanyLogoUrl);