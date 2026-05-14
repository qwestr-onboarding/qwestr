namespace Qwestr.Shared.Contracts.Identity;

public interface ITenantProvider
{
    Task<TenantDto?> GetByIdAsync(Guid tenantId, CancellationToken ct = default);
}
