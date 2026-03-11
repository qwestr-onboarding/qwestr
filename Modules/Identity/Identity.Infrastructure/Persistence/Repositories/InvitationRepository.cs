using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Identity.Application.Abstractions.Repositories;
using Qwestr.Modules.Identity.Domain.Entities;
using Qwestr.Modules.Identity.Domain.Enums;

namespace Qwestr.Modules.Identity.Infrastructure.Persistence.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly IdentityDbContext _context;

    public InvitationRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Invitation?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _context.Invitations.FirstOrDefaultAsync(i => i.Token == token, ct);

    public async Task<List<Invitation>> GetByTenantAsync(Guid tenantId, int page, int pageSize, CancellationToken ct = default)
        => await _context.Invitations
            .Where(i => i.TenantId == tenantId)
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<bool> ExistsPendingAsync(Guid tenantId, string email, CancellationToken ct = default)
        => await _context.Invitations.AnyAsync(
            i => i.TenantId == tenantId && i.Email == email && i.Status == InvitationStatus.Pending, ct);

    public async Task AddAsync(Invitation invitation, CancellationToken ct = default)
        => await _context.Invitations.AddAsync(invitation, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
