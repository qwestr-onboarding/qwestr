namespace Qwestr.Modules.Identity.Application.DTOs;

public record TenantSettingsResponse(
    string CompanyName,
    string? CompanyLogoUrl,
    string Timezone,
    string Language);
