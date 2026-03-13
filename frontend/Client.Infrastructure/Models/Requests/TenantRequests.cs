namespace Client.Infrastructure.Models.Requests;

public record UpdateTenantSettingsRequest(
    string? Timezone,
    string? Language,
    string? CompanyLogoUrl);