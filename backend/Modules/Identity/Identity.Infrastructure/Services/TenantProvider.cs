using Microsoft.EntityFrameworkCore;
using Qwestr.Shared.Contracts.Identity;

namespace Qwestr.Modules.Identity.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private readonly IdentityDbContext _context;

    public TenantProvider(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDto?> GetByIdAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _context.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => new TenantDto(t.Id, t.Name, t.Subdomain))
            .FirstOrDefaultAsync(ct);
    }
}
