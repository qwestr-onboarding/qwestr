namespace Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;

public record TenantSettingsResponse(
    string CompanyName,
    string? CompanyLogoUrl,
    string Timezone,
    string Language);
