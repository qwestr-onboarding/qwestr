using Qwestr.Modules.Identity.Domain.Enums;

namespace Qwestr.Modules.Identity.Application.DTOs;

public record TenantInfoResponse(
    Guid Id,
    string Name,
    string? Subdomain,
    TenantPlan Plan,
    bool IsActive,
    DateTime CreatedAt);
