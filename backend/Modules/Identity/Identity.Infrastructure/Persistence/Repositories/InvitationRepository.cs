using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Identity;

namespace Qwestr.Modules.Identity.Infrastructure.Persistence.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly IdentityDbContext _context;

    public InvitationRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Invitations.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<Invitation?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _context.Invitations.FirstOrDefaultAsync(i => i.Token == token, ct);

    public async Task<Invitation?> GetByTokenWithTenantAsync(string token, CancellationToken ct = default)
        => await _context.Invitations
            .Include(i => i.Tenant)
            .FirstOrDefaultAsync(i => i.Token == token, ct);

    public async Task<List<Invitation>> GetByTenantAsync(Guid tenantId, int page, int pageSize, InvitationStatus? status = null, CancellationToken ct = default)
        => await _context.Invitations
            .Include(i => i.InvitedByUser)
            .Where(i => i.TenantId == tenantId && (status == null || i.Status == status))
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountByTenantAsync(Guid tenantId, InvitationStatus? status = null, CancellationToken ct = default)
        => await _context.Invitations
            .CountAsync(i => i.TenantId == tenantId && (status == null || i.Status == status), ct);

    public async Task<bool> ExistsPendingAsync(Guid tenantId, string email, CancellationToken ct = default)
        => await _context.Invitations.AnyAsync(
            i => i.TenantId == tenantId && i.Email == email && i.Status == InvitationStatus.Pending, ct);

    public async Task AddAsync(Invitation invitation, CancellationToken ct = default)
        => await _context.Invitations.AddAsync(invitation, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
