using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Shared.Contracts.ApiDto.Identity.Tenant.Responses;

public record TenantInfoResponse(
    Guid Id,
    string Name,
    string? Subdomain,
    TenantPlan Plan,
    bool IsActive,
    DateTime CreatedAt);
