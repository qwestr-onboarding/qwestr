using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;

namespace Qwestr.Modules.Identity.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly IdentityDbContext _context;

    public TenantRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken ct = default)
        => await _context.Tenants.FirstOrDefaultAsync(t => t.Subdomain == subdomain, ct);

    public async Task<bool> ExistsBySubdomainAsync(string subdomain, CancellationToken ct = default)
        => await _context.Tenants.AnyAsync(t => t.Subdomain == subdomain, ct);

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
        => await _context.Tenants.AddAsync(tenant, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
