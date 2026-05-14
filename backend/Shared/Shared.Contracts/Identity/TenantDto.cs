namespace Qwestr.Shared.Contracts.Identity;

public record TenantDto(Guid Id, string Name, string? Subdomain);
